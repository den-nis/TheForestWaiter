using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Core
{
	/// <summary>
	/// Movable objects can move and can interact with the world
	/// </summary>
	internal abstract class Movable : GameObject
    {
        private const int RADIUS_CHECK_MARGIN = 50;
        private const int TERMINAL_VELOCITY = 500;
        private const int DEFAULT_GRAVITY = 900;

        public Vector2f Velocity { get; set; }
        public WorldCollisionFlags CollisionFlags { get; private set; }

        protected Vector2f Drag { get; set; } = new Vector2f(0, 0);
        protected float Gravity { get; set; } = DEFAULT_GRAVITY;

        /// <summary>
        /// Enable multiple collision checks/moves in a single update for more accuracy
        /// </summary>
        protected bool DeepCollision { get; set; } = true;
        protected bool EnableCollision { get; set; } = true;

        private Vector2f _movingSpeed;
        private Vector2f _lastWorldCheckArea;
        private Vector2f _lastWorldCheckCenter;

        public Movable(GameData game) : base(game) 
        {
        }

        public override void Update(float time)
        {
            UpdatePhysics(time);
        }

        public Vector2f GetSpeed() => _movingSpeed;

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

		private void UpdatePhysics(float time)
        {
            CollisionFlags = WorldCollisionFlags.None;

            ApplyGravity(time);
            ApplyDrag(time);

            var previousPosition = Position; 
            Move(Velocity * time);
            _movingSpeed = (Position - previousPosition) / time;
        }

        public void LimitPush(Vector2f topSpeed, Vector2f acceleration, float time)
        {
            SetVelocityX(LimitPush1D(Velocity.X, acceleration.X, topSpeed.X, time));
            SetVelocityY(LimitPush1D(Velocity.Y, acceleration.Y, topSpeed.Y, time));
        }

        private static float LimitPush1D(float currentSpeed, float acceleration, float topSpeed, float time)
        {
            float newSpeed = currentSpeed + acceleration * time;

            if (Math.Abs(newSpeed) < Math.Abs(currentSpeed))
                return newSpeed;

            if (Math.Abs(newSpeed) > Math.Abs(topSpeed))
            {
                if (Math.Abs(currentSpeed) < Math.Abs(topSpeed))
                {
                    return (float)Math.CopySign(topSpeed, newSpeed);
				}

                return currentSpeed;
            }

            return newSpeed;
        }

        private void ApplyGravity(float time)
        {
            if (Velocity.Y < TERMINAL_VELOCITY)
            {
                Push(new Vector2f(0, Gravity), time);
            }
        }

        private void ApplyDrag(float time)
        {
            if (Drag.X > 0 || Drag.Y > 0)
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
            CollisionFlags = WorldCollisionFlags.None;
            if (!EnableCollision)
			{
                Position += move;
                return;
			}

            float remainingTime = float.MaxValue;

            while (remainingTime > 0)
            {
                float nearestTime = GetNearestSweptAABB(move, out Vector2f normal);
                remainingTime = 1.0f - nearestTime;

                Position += move * nearestTime;

                if (nearestTime != 1.0f)
                {
                    CollisionFlags |= GetCollisionFlags(normal);

                    if (CollisionFlags.HasFlag(WorldCollisionFlags.Horizontal))
                        SetVelocityX(0);

                    if (CollisionFlags.HasFlag(WorldCollisionFlags.Vertical))
                        SetVelocityY(0);

                    if (DeepCollision)
                    {
                        float dotprod = (move.X * normal.Y + move.Y * normal.X) * remainingTime;
                        move = new Vector2f(dotprod * normal.Y, dotprod * normal.X);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private float GetNearestSweptAABB(Vector2f move, out Vector2f normal)
        {
            float checkRadius = Math.Max(Size.X, Size.Y) + RADIUS_CHECK_MARGIN;
            float nearestTime = 1;
            Vector2f nearestNormal = default;

            var area = new Vector2f(
                    checkRadius + Math.Abs(move.X),
                    checkRadius + Math.Abs(move.Y)
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
                    out Vector2f norm);

                if (time < nearestTime)
                {
                    nearestNormal = norm;
                    nearestTime = time;
                }
            }

            normal = nearestNormal;
            return nearestTime;
        }

        private static WorldCollisionFlags GetCollisionFlags(Vector2f normal)
        {
            WorldCollisionFlags flags = WorldCollisionFlags.None;

            if (Math.Abs(normal.X) > 0) flags |= WorldCollisionFlags.Horizontal;
            if (Math.Abs(normal.Y) > 0) flags |= WorldCollisionFlags.Vertical;
            if (normal.Y > 0) flags |= WorldCollisionFlags.Top;
            if (normal.Y < 0) flags |= WorldCollisionFlags.Bottom;
            if (normal.X < 0) flags |= WorldCollisionFlags.Left;
            if (normal.X > 0) flags |= WorldCollisionFlags.Right;

            return flags;
        }

        public override void DrawHitbox(RenderWindow window, float lineThickness)
        {
            base.DrawHitbox(window, lineThickness);
            var position = _lastWorldCheckCenter - _lastWorldCheckArea / 2;
            window.DrawHitBox(position, _lastWorldCheckArea, Color.Blue, lineThickness);
        }
    }
}