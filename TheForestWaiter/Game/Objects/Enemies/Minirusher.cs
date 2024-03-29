﻿using SFML.Graphics;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Game.Objects.Enemies
{
	internal class Minirusher : GroundCreature
	{
		private const float ATTACK_DAMAGE = 5;
		private const float KNOCKBACK = 100;

		private readonly AnimatedSprite _animation;
		private readonly RandomTrigger _jumpTrigger;
		private readonly ContentSource _content;
		private readonly DropSpawner _dropSpawner;

		public Minirusher()
		{
			_content = IoC.GetInstance<ContentSource>();
			_dropSpawner = IoC.GetInstance<DropSpawner>();

			_animation = _content.Textures.CreateAnimatedSprite("Textures/Enemies/minirusher.png");
			Size = _animation.Sheet.TileSize.ToVector2f();

			_dropSpawner.Setup("Textures/Enemies/minirusher_gibs.png");
			_dropSpawner.ChanceOfHeartDrop = 0.01f;

			SetMaxHealth(100, true);
			UseHoldJumpWhenChase = true;
			WalkSpeed = 120 + Rng.Range(0, 100);
			JumpForce = 350;
			JumpForceVariation = 200;
			AirSpeed = 100;
			AirAcceleration = 500;
			Acceleration = 3000;

			_jumpTrigger = new RandomTrigger(Jump, 70, 1, 2);
		}

		public override void Update(float time)
		{
			base.Update(time);
			_jumpTrigger.Update(time);

			Chase(Game.Objects.Player);
			if (Intersects(Game.Objects.Player))
			{
				Game.Objects.Player.Damage(this, ATTACK_DAMAGE, KNOCKBACK);
			}

			HandleAnimations(time);
		}

		public void HandleAnimations(float time)
		{
			_animation.Sprite.Color = IsStunned ? new Color(255, 0, 0) : Color.White;
			_animation.Sprite.Position = Position;

			var playerDirection = Math.Sign(Game.Objects.Player.Center.X - Center.X);

			if (FacingDirection == 0)
			{
				_animation.SetSection("idle");
			}
			else
			{
				if (playerDirection > 0)
					_animation.Sheet.MirrorX = true;

				if (playerDirection < 0)
					_animation.Sheet.MirrorX = false;

				_animation.SetSection("walking");
			}

			_animation.Update(time);
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_animation);
		}

		protected override void OnDeath()
		{
			_dropSpawner.Spawn(Center);
			Delete();
		}

		protected override void OnDamage(GameObject by, float amount)
		{
			var prop = _content.Particles.Get("Particles/blood.particle", Center);
			Game.Objects.WorldParticles.Emit(prop, (int)(5 + (amount / 100)));
		}
	}
}
