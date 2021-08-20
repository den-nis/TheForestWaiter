using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TheForestWaiter.Debugging;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Objects;

namespace TheForestWaiter.Entities
{
    /// <summary>
    /// Dynamic objects have physics and can interact with the world
    /// </summary>
    abstract class DynamicObject : GameObject
    {
        public bool ReceiveDynamicCollisions { get; set; } = true;
        public bool EmitDynamicCollisions { get; set; } = true;
        public bool EnableWorldCollisions { get; set; } = true;

        private const int TERMINAL_VELOCITY = 500; 

        public DynamicObject(GameData game) : base(game) { }

        private float LastDelta { get; set; }
        private Vector2f LastFramePosition { get; set; }

        //Physics
        public float CollisionRadius { get; set; } = 50;
        public float Gravity { get; set; } = 0;
        public float Mass { get; set; } = 100;
        public Vector2f velocity;
        public Vector2f RealSpeed { get; set; }
        public Vector2f Drag { get; set; } = new Vector2f(0, 0);

        //Touching
        public bool TouchingHorizontal { get; private set; } = false;
        public bool TouchingVertical { get; private set; } = false;
        public bool TouchingFloor { get; private set; } = false;
        public bool TouchingCeiling { get; private set; } = false;

        public bool RespondToCollision { get; set; } = true;

        public override void Update(float time)
        {
            RealSpeed = (Position - LastFramePosition) / LastDelta;
            LastFramePosition = Position;
            LastDelta = time;

            GameDebug.DrawHitBox(Position, Size, Color.Blue);

            ApplyGravity(time);
            ApplyDrag(time);

            Move(velocity * time);

            HandleDynamicCollisions(time);
        }

        public void Push(Vector2f force, float time)
        {
            velocity += force * time;
        }

        public void LimitPush(Vector2f force, float time)
        {
            velocity.X = LimitPush1D(velocity.X, force.X, time);
            velocity.Y = LimitPush1D(velocity.Y, force.Y, time);
        }

        private static float LimitPush1D(float speed, float push, float time)
        {
            if (Math.Sign(speed) == Math.Sign(push))
            {
                if (Math.Abs(speed) < Math.Abs(push))
                {
                    speed = Math.Min(
                        Math.Abs(push),
                        Math.Abs(speed) + Math.Abs(push) * time) * Math.Sign(speed);
                }
                return speed;
            }
            return speed + push * time;
        }

        protected virtual void OnTouch(DynamicObject obj) {}

        private void HandleDynamicCollisions(float time)
        {
            if (!EmitDynamicCollisions)
                return;

            foreach(var obj in Game.Objects.Creatures)
            {
                if (Intersects(obj) && 
                    obj.GameObjectId != GameObjectId && 
                    obj.ReceiveDynamicCollisions)
                {
                    OnTouch(obj);
                    PushAway(obj, time);
                }
            }
        }

        private void PushAway(DynamicObject obj, float time)
        {
            var push = obj.Center - Center;

            if (push.Len() < 0.05f)
                push = new Vector2f(Rng.Float() * 2 - 1, Rng.Float() * 2 - 1);

            obj.Push(push * 100, time);
        }

        private void ApplyGravity(float time)
        {
            if (velocity.Y < TERMINAL_VELOCITY)
                velocity.Y += Gravity * time;
        }

        private void ApplyDrag(float time)
        {
            var newVelocity = new Vector2f(
                velocity.X - (float)Math.CopySign(Drag.X, velocity.X) * time,
                velocity.Y - (float)Math.CopySign(Drag.Y, velocity.Y) * time
            );

            if (Math.Sign(newVelocity.X) != Math.Sign(velocity.X))
                newVelocity.X = 0;

            if (Math.Sign(newVelocity.Y) != Math.Sign(velocity.Y))
                newVelocity.Y = 0;

            velocity = newVelocity;
        }

        private void Move(Vector2f move)
        {
            if (!EnableWorldCollisions)
			{
                Position += move;
                return;
			}

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

            var area = new Vector2f(
                    CollisionRadius + Math.Abs(move.X),
                    CollisionRadius + Math.Abs(move.Y)
                );
            var center = Center + move/2;

            var tiles = Game.World.GetTilesInArea(area, center);

            GameDebug.DrawHitBox(center - (area / 2), area, Color.Green);
            foreach (var tile in tiles)
            {
                GameDebug.DrawWorldCollison(tile.Position);

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
