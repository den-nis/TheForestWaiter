﻿using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Core;

namespace TheForestWaiter.Game.Objects
{
	/// <summary>
	/// Creature with movement logic
	/// </summary>
	internal abstract class GroundCreature : Creature
	{
		protected float JumpForce { get; set; } = 500;
		protected float JumpHoldForce { get; set; } = 100;
		protected float JumpForceVariation { get; set; } = 100;
		protected float WalkSpeed { get; set; } = 250;
		protected float Acceleration { get; set; } = 3000;
		protected float Friction { get; set; } = 500;
		protected float? AirSpeed { get; set; } = null;
		protected float? AirAcceleration { get; set; } = null;
		protected bool AllowAirMovement { get; set; } = true;
		protected bool AutoJumpObstacles { get; set; } = true;
		protected bool DisableDefaultMovementLogic { get; set; } = false;
		protected bool UseHoldJumpWhenChase { get; set; } = false;

		private int _targetMovingDirection = 0;
		private bool _wantsToJump = false;
		private bool _wantsToHoldJump = false;

		public GroundCreature(GameData game) : base(game)
		{
			Drag = new Vector2f(0, 0);
		}

		public override void Update(float time)
		{
			base.Update(time);

			if (!DisableDefaultMovementLogic)
			{
				PerformMovement(time);
			}
		}

		protected void Chase(GameObject target)
		{
			var targetXDirection = Math.Sign(target.Center.X - Center.X);
			var targetYDirection = Math.Sign(target.Center.Y - Center.Y);

			if (targetYDirection < 0 && UseHoldJumpWhenChase)
			{
				HoldJump();
			}

			MoveDirection(targetXDirection);
		}

		protected void MoveDirection(int direction)
		{
			if (direction > 0)
				MoveRight();

			if (direction < 0)
				MoveLeft();
		}

		protected void MoveLeft() => _targetMovingDirection = -1;

		protected void MoveRight() => _targetMovingDirection = 1;
		
		protected void HoldJump() => _wantsToHoldJump = true;
		
		protected void Jump() => _wantsToJump = true;
		
		private void PerformMovement(float time)
		{
			bool jumpNextUpdate = false;
			bool grounded = CollisionFlags.HasFlag(WorldCollisionFlags.Bottom);
			float speed = grounded ? WalkSpeed : (AirSpeed ?? WalkSpeed);
			float acceleration = grounded ? Acceleration : (AirAcceleration ?? Acceleration);

			if (_targetMovingDirection == 0)
			{
				Drag = new Vector2f(Friction, Drag.Y);
			}
			else
			{
				Drag = new Vector2f(0, Drag.Y);
				if (_targetMovingDirection > 0)
				{
					HorizontalSpeed(speed, acceleration, time);
				}

				if (_targetMovingDirection < 0)
				{
					HorizontalSpeed(-speed, acceleration, time);
				}

				if (CollisionFlags.HasFlag(WorldCollisionFlags.Horizontal) && AutoJumpObstacles)
				{
					jumpNextUpdate = true;
				}
			}
			
			if (_wantsToJump)
			{
				if (grounded)
				{
					SetVelocityY(-JumpForce + Rng.Range(-JumpForceVariation, JumpForceVariation));
				}
			}

			if (_wantsToHoldJump)
			{
				Push(new Vector2f(0, -JumpHoldForce), time);
			}

			_targetMovingDirection = 0;
			_wantsToHoldJump = false;
			_wantsToJump = jumpNextUpdate;
		}

		private void HorizontalSpeed(float speed, float acceleration, float time)
		{
			LimitPush(new Vector2f(speed, 0), new Vector2f(acceleration * Math.Sign(speed), 0), time);
		}
	}
}
