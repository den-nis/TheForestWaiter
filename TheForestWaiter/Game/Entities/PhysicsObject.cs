using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Essentials;
using LightInject;
using System.Linq;

namespace TheForestWaiter.Game.Entities
{
    /// <summary>
    /// Physics objects have physics and can interact with the world
    /// </summary>
    abstract class PhysicsObject : GameObject
    {
        public static bool DrawHitBox { get; set; } = false;
        public static bool DrawWorldCollisionBox { get; set; } = false;

        public bool ReceivePhysicsCollisions { get; set; } = true;
        public bool EmitPhysicsCollisions { get; set; } = true;
        public bool EnableWorldCollisions { get; set; } = true;

        private const int MAX_PHYSICS_TOUCHES = 10;
        private const int TERMINAL_VELOCITY = 500; 

        public PhysicsObject(GameData game) : base(game) 
        {
            
        }

        private Vector2f _lastWorldCheckArea;
        private Vector2f _lastWorldCheckCenter;

        //Physics
        public float CollisionRadius { get; set; }
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

        public bool RespondToWorldCollision { get; set; } = true;

        public void PhysicsTick(float time)
        {

            var previousPosition = Position; 

            ApplyGravity(time);
            ApplyDrag(time);

            Move(velocity * time);
            HandlePhysicsCollisions(time);

            RealSpeed = Position - previousPosition;
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

        protected virtual void OnTouch(PhysicsObject obj) {}

        private void HandlePhysicsCollisions(float time)
        {
            if (!EmitPhysicsCollisions)
                return;

            int touch = 0;
            foreach(var obj in Game.Objects.Creatures)
            {
                if (touch >= MAX_PHYSICS_TOUCHES)
                    break;
                
                if (Intersects(obj) && 
                    obj.GameObjectId != GameObjectId && 
                    obj.ReceivePhysicsCollisions)
                {
                    OnTouch(obj);
                    PushAway(obj, time);
                    touch++;
                }
            }
        }

        private void PushAway(PhysicsObject obj, float time)
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

                    if (!RespondToWorldCollision)
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

            _lastWorldCheckCenter = center;
            _lastWorldCheckArea = area;

            foreach (var tile in tiles)
            {
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

        public override void DrawHitbox(RenderWindow window, float lineThickness)
        {
            base.DrawHitbox(window, lineThickness);
            var position = _lastWorldCheckCenter - _lastWorldCheckArea / 2;
            window.DrawHitBox(position, _lastWorldCheckArea, Color.Blue, lineThickness);
        }
    }
}
