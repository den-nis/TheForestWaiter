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

namespace TheForestWaiter.Game.Core
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

        private const int TERMINAL_VELOCITY = 500; 

        public PhysicsObject(GameData game) : base(game) 
        {
            
        }

        private Vector2f _lastWorldCheckArea;
        private Vector2f _lastWorldCheckCenter;

        //Physics
        public float CollisionRadius { get; set; }
        public float Gravity { get; set; } = 1000;

        public Vector2f Velocity { get; set; }
        public Vector2f RealSpeed { get; set; }
        public Vector2f Drag { get; set; } = new Vector2f(500, 0);
        public bool EnableDrag { get; set; }

        //Touching
        public bool TouchingHorizontal { get; private set; } = false;
        public bool TouchingVertical { get; private set; } = false;
        public bool TouchingFloor { get; private set; } = false;
        public bool TouchingCeiling { get; private set; } = false;

        public bool RespondToWorldCollision { get; set; } = true;

        public void PhysicsTick(float time)
        {
            var previousPosition = Position; 

            HandlePhysicsCollisions(time);

            ApplyGravity(time);
            ApplyFriction(time);

            Move(Velocity * time);

            RealSpeed = (Position - previousPosition) / time;
        }

        public void SetVelocityX(float velocity)
        {
            Velocity = new Vector2f(velocity, Velocity.Y);
        }

        public void SetVelocityY(float velocity)
        {
            Velocity = new Vector2f(Velocity.X, velocity);
        }

        public void Push(Vector2f force, float time)
        {
            Velocity += force * time;
        }

        public void LimitPush(Vector2f force, float time)
        {
            SetVelocityX(LimitPush1D(Velocity.X, force.X, time));
            SetVelocityY(LimitPush1D(Velocity.Y, force.Y, time));
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
            {
                return;
            }

            int touch = 0;
            foreach(var obj in Game.Objects.PhysicsObjects)
            {
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

        ////TODO: refactor. force can be a constant etc
        private void PushAway(PhysicsObject obj, float time)
        {
            var distance = (obj.Center - Center).Len();
            var force = 500;

            if (distance < 5f)
            {
                obj.LimitPush(TrigHelper.FromAngleRad(Rng.Angle(), force), time);
            }
            else
            {
                var angle = (obj.Center - Center).Angle();
                var push = TrigHelper.FromAngleRad(angle, force);
                obj.LimitPush(push, time);
            }
        }

        private void ApplyGravity(float time)
        {
            if (Velocity.Y < TERMINAL_VELOCITY)
            {
                Push(new Vector2f(0, Gravity), time);
            }
        }

        private void ApplyFriction(float time)
        {
            if (EnableDrag)
            {
                var newVelocity = new Vector2f(
                    Velocity.X - (float)Math.CopySign(Drag.X, Velocity.X) * time,
                    Velocity.Y - (float)Math.CopySign(Drag.Y, Velocity.Y) * time
                );

                if (Math.Sign(newVelocity.X) != Math.Sign(Velocity.X))
                    newVelocity.X = 0;

                if (Math.Sign(newVelocity.Y) != Math.Sign(Velocity.Y))
                    newVelocity.Y = 0;

                Velocity = newVelocity;
            }
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
                SetVelocityX(0);

            if (TouchingVertical)
                SetVelocityY(0);
        }

        private float GetNearestSweptAABB(Vector2f move, out Vector2f normal)
        {
            float nearestTime = 1;
            Vector2f nearestNormal = default;

            var area = new Vector2f(
                    CollisionRadius + Math.Abs(move.X),
                    CollisionRadius + Math.Abs(move.Y)
                );
            var center = Center + move / 2;

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
