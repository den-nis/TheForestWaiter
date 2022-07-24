using SFML.System;
using System;

namespace TheForestWaiter.Game.Objects.Abstract
{
	/// <summary>
	/// Creature with movement logic
	/// </summary>
	internal abstract class FlyingCreature : Creature
	{
		protected float Acceleration { get; set; } = 700;
		protected float Speed { get; set; } = 250;
		protected float FlyingHeight { get; set; } = 200;
		protected bool EnableHovering { get; set; } = true;

		private int _targetMovingDirectionX = 0;
		private int _targetMovingDirectionY = 0;

		public FlyingCreature()
		{
			Drag = new Vector2f(0, 0);
			Gravity = 0;
		}

		public override void Update(float time)
		{
			base.Update(time);
			PerformMovement(time);
		}

		protected void Chase(GameObject target)
		{
			var targetXDirection = Math.Sign(target.Center.X - Center.X);
			MoveDirection(targetXDirection);
		}

		protected void MoveDirection(int direction)
		{
			if (direction > 0)
				MoveRight();

			if (direction < 0)
				MoveLeft();
		}

		protected void MoveLeft() => _targetMovingDirectionX = -1;

		protected void MoveRight() => _targetMovingDirectionX = 1;

		protected void FlyUp() => _targetMovingDirectionY = -1;

		protected void FlyDown() => _targetMovingDirectionY = 1;

		//TODO: refactor. this not the most reliable way to get the height.
		private float GetHeight()
		{
			Vector2f position = Center;
			for (int i = 0; i < 1000; i++)
			{
				position += new Vector2f(0, 16);
				if (Game.World.TouchingSolid(position))
				{
					var tile = Game.World.GetTileInfoAt(position);
					return tile.Value.Position.Y - Center.Y;
				}
			}

			return float.PositiveInfinity;
		}

		private void PerformMovement(float time)
		{
			var height = GetHeight();
			if (EnableHovering)
			{
				if (height > FlyingHeight)
				{
					FlyDown();
				}
				else
				{
					FlyUp();
				}
			}

			if (_targetMovingDirectionX != 0)
			{
				var sign = Math.Sign(_targetMovingDirectionX);
				LimitPush(new Vector2f(Speed * sign, 0), new Vector2f(Acceleration * sign, 0), time);
			}

			if (_targetMovingDirectionY != 0)
			{
				var sign = Math.Sign(_targetMovingDirectionY);
				LimitPush(new Vector2f(0, Speed * sign), new Vector2f(0, Acceleration * sign), time);
			}

			_targetMovingDirectionX = 0;
			_targetMovingDirectionY = 0;
		}
	}
}
