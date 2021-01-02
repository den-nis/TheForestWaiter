using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TheForestWaiter.Debugging;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;

namespace TheForestWaiter.Entites
{
    /// <summary>
    /// Dynamic objects have physics and can interact with the world
    /// </summary>
    abstract class DynamicObject : GameObject
    {
        public DynamicObject(GameData game) : base(game) { }

        private float LastDelta { get; set; }
        private Vector2f LastFramePosition { get; set; }

        //Physics
        public float CollisionRadius { get; set; } = 200;
        public float Gravity { get; set; } = 0;
        public Vector2f velocity;
        public Vector2f Drag { get; set; } = new Vector2f(0, 0);
        public float Mass { get; set; } = 100;
        public Vector2f RealSpeed => (Position - LastFramePosition) / LastDelta;

        //Touching
        public bool TouchingHorizontal { get; private set; } = false;
        public bool TouchingVertical { get; private set; } = false;
        public bool TouchingFloor { get; private set; } = false;
        public bool TouchingCeiling { get; private set; } = false;

        public bool RespondToCollision { get; set; } = true;

        public override void Update(float time)
        {
            LastFramePosition = Position;
            LastDelta = time;

            ApplyGravity(time);
            ApplyDrag(time);

            Move(velocity * time);

            GameDebug.UpdateHitBox(Position, Size);
        }

        private void ApplyGravity(float time)
        {
            if (velocity.Y < 500)
                velocity.Y += Gravity * time;
        }

        private void ApplyDrag(float time)
        {
            velocity = new Vector2f(
                velocity.X - (float)Math.CopySign(Drag.X, velocity.X) * time,
                velocity.Y - (float)Math.CopySign(Drag.Y, velocity.Y) * time
            );

            if (Math.Abs(velocity.X) <= Drag.X * time)
                velocity.X = 0;

            if (Math.Abs(velocity.Y) <= Drag.Y * time)
                velocity.Y = 0;
        }

        private void Move(Vector2f move)
        {
            float remainingTime = float.MaxValue;

            ResetStaticBoxTouching();
            while (remainingTime > 0)
            {
                float nearestTime = GetNearestSweptAABB(move, out Vector2f normal);
                remainingTime = 1.0f - nearestTime;

                Position += move * nearestTime;

                if (nearestTime != 1.0f)
                {
                    HandleStaticBoxTouching(normal);

                    if (!RespondToCollision)
                        break;

                    float dotprod = (move.X * normal.Y + move.Y * normal.X) * remainingTime;
                    move = new Vector2f(dotprod * normal.Y, dotprod * normal.X);
                }
            }
        }

        private void ResetStaticBoxTouching()
        {
            TouchingHorizontal = false;
            TouchingVertical = false;
            TouchingCeiling = false;
            TouchingFloor = false;
        }

        private void HandleStaticBoxTouching(Vector2f normal)
        {
            TouchingHorizontal |= Math.Abs(normal.X) > 0;
            TouchingVertical |= Math.Abs(normal.Y) > 0;
            TouchingCeiling |= normal.Y > 0;
            TouchingFloor |= normal.Y < 0;

            if (TouchingHorizontal)
                velocity.X = 0;

            if (TouchingVertical)
                velocity.Y = 0;
        }

        private float GetNearestSweptAABB(Vector2f move, out Vector2f normal)
        {
            float nearestTime = 1;
            Vector2f nearestNormal = default;

            var tiles = Game.World.GetTilesInArea(
                new Vector2f(
                    Math.Abs(RealSpeed.X) + CollisionRadius,
                    Math.Abs(RealSpeed.Y) + CollisionRadius
                ),
                Center);

            foreach (var tile in tiles)
            {
                GameDebug.RegisterWorldCollisonCheck(tile.Position);

                float time = Collisions.SweptAABB(
                    new FloatRect(tile.Position, new Vector2f(World.TILE_SIZE, World.TILE_SIZE)), 
                    new FloatRect(Position, Size),
                    move, 
                    out Vector2f n);

                if (time < nearestTime)
                {
                    nearestNormal = n;
                    nearestTime = time;
                }
            }

            normal = nearestNormal;
            return nearestTime;
        }
    }
}
