using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Game.Objects.Enemies
{
	internal class Bigrusher : GroundCreature
	{
		private const float ATTACK_DAMAGE = 10;
		private const float KNOCKBACK = 100;
		private const int SPAWN_MINIONS = 50;

		private readonly AnimatedSprite _animation;
		private readonly RandomTrigger _jumpTrigger;
		private readonly ContentSource _content;
		private readonly DropSpawner _dropSpawner;
		private readonly ObjectCreator _creator;

		public Bigrusher()
		{
			_content = IoC.GetInstance<ContentSource>();
			_dropSpawner = IoC.GetInstance<DropSpawner>();
			_creator = IoC.GetInstance<ObjectCreator>();

			_animation = _content.Textures.CreateAnimatedSprite("Textures/Enemies/bigrusher.png");
			Size = _animation.Sheet.Rect.CellSize.ToVector2f();

			_dropSpawner.Setup("Textures/Enemies/bigrusher_gibs.png");
			_dropSpawner.ChanceOfHeartDrop = 0.01f;
			_dropSpawner.MaxAmountCoins = 25;

			SetMaxHealth(3000, true);
			UseHoldJumpWhenChase = true;
			Gravity = 300;
			WalkSpeed = 120;
			JumpForce = 200;
			JumpForceVariation = 200;
			AirSpeed = 100;
			AirAcceleration = 500;
			Acceleration = 300;
			KnockbackResistance = 10000;

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
					_animation.Sheet.Rect.MirrorX = true;

				if (playerDirection < 0)
					_animation.Sheet.Rect.MirrorX = false;

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

			for (int i = 0; i < SPAWN_MINIONS; i++)
			{
				var at = Center + new Vector2f(Rng.Var(40), Rng.Var(40));
				var speed = (at - Center).Norm() * 400;

				var obj = _creator.CreateAndShoot<Minirusher>(at, speed);
				Game.Objects.QueueAddGameObject(obj);
			}

			Delete();
		}

		protected override void OnDamage(GameObject by, float amount)
		{
		}
	}
}
