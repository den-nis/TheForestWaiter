using SFML.Graphics;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Logic;

namespace TheForestWaiter.Game.Objects.Enemies
{
	internal class Minirusher : GroundCreature
	{
		private const float ATTACK_DAMAGE = 10;
		private const float KNOCKBACK = 100;

		private readonly AnimatedSprite _animation;
		private readonly RandomTrigger _jumpTrigger;
		private readonly ContentSource _content;
		private readonly DropSpawner _dropSpawner;

		public Minirusher(GameData game, ContentSource content, DropSpawner dropSpawner) : base(game)
		{
			_animation = content.Textures.CreateAnimatedSprite("Textures/Enemies/minirusher.png");
			Size = _animation.Sheet.TileSize.ToVector2f();

			dropSpawner.Setup("Textures/Enemies/minirusher_gibs.png");
			dropSpawner.ChanceOfHeartDrop = 0.01f;

			SetMaxHealth(10, true);
			UseHoldJumpWhenChase = true;
			WalkSpeed = 220 + Rng.Range(0, 100);
			JumpForce = 350;
			JumpForceVariation = 200;
			AirSpeed = 100;
			AirAcceleration = 500;
			Acceleration = 3000;

			_jumpTrigger = new RandomTrigger(Jump, 70, 1, 2);
			_content = content;
			_dropSpawner = dropSpawner;
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

		protected override void OnDamage(GameObject by)
		{
			var prop = _content.Particles.Get("Particles/blood.particle", Center);
			Game.Objects.WorldParticles.Emit(prop, 5);
		}
	}
}
