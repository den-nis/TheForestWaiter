using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Objects.Projectiles;

namespace TheForestWaiter.Game.Objects.Enemies
{
	internal class Crawler : GroundCreature
	{
		private const float CHASE_DISTANCE = 200;
		private const float AVOID_DISTANCE = 100;
		private const float AVOID_DISTANCE_VARIATION = 20;

		private const float BULLET_VELOCITY = 300;
		private const float ATTACK_PREPARE_TIME = 1f;

		private readonly AnimatedSprite _animation;
		private readonly ContentSource _content;
		private readonly DropSpawner _dropSpawner;
		private readonly ObjectCreator _creator;

		private int PlayerDirection => Math.Sign(Game.Objects.Player.Center.X - Center.X);
		private int PlayerHeightDifference => (int)Math.Abs(Game.Objects.Player.Center.Y - AttackPoint.Y);

		private Vector2f AttackPoint => Center + new Vector2f(PlayerDirection * 15, 0);

		private readonly RandomTrigger _attackTrigger;
		private float _prepareAttackTime = 0;

		private readonly float _avoidDistance;

		public Crawler()
		{
			_creator = IoC.GetInstance<ObjectCreator>();
			_content = IoC.GetInstance<ContentSource>();
			_dropSpawner = IoC.GetInstance<DropSpawner>();

			_animation = _content.Textures.CreateAnimatedSprite("Textures/Enemies/crawler.png");
			Size = _animation.Sheet.TileSize.ToVector2f();

			SetMaxHealth(300, true);
			JumpForce = Rng.Var(400, variation: 20);
			WalkSpeed = Rng.Var(120, variation: 10);

			_avoidDistance = Rng.Var(AVOID_DISTANCE, AVOID_DISTANCE_VARIATION);
			_attackTrigger = new RandomTrigger(PrepareAttack, 0.1f, 0.1f);
			_dropSpawner.Setup("Textures/Enemies/crawler_gibs.png");
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_animation);
		}

		public override void Update(float time)
		{
			base.Update(time);
			HandleAnimations(time);

			if (_prepareAttackTime > 0)
			{
				_prepareAttackTime -= time;
				if (_prepareAttackTime <= 0)
				{
					Attack();
				}
			}

			var distance = Math.Abs(Game.Objects.Player.Center.X - Center.X);
			if (distance > CHASE_DISTANCE || PlayerHeightDifference > 9)
			{
				Chase(Game.Objects.Player);
			}
			else if (distance < _avoidDistance)
			{
				MoveDirection(PlayerDirection * -1);
			}

			_attackTrigger.Update(time);
		}

		public void HandleAnimations(float time)
		{
			_animation.Sprite.Color = IsStunned ? new Color(255, 0, 0) : Color.White;
			_animation.Sprite.Position = Position;
			_animation.Sheet.MirrorX = PlayerDirection == 1;

			if (_prepareAttackTime > 0 && false)
			{
				_animation.SetSection("attack");
			}
			else
			{
				if (Math.Abs(GetSpeed().X) > 0.5f)
				{
					_animation.SetSection("walking");
				}
				else
				{
					_animation.SetSection("idle");
				}
			}

			_animation.Update(time);
		}

		private void PrepareAttack()
		{
			if (_prepareAttackTime <= 0 && PlayerHeightDifference < 10)
			{
				_prepareAttackTime = ATTACK_PREPARE_TIME;
			}
		}

		private void Attack()
		{
			var velocity = new Vector2f(BULLET_VELOCITY * PlayerDirection, 0);
			var bullet = _creator.FireProjectile<CorruptionBall>(AttackPoint, velocity, this);
			Game.Objects.AddGameObject(bullet);
		}

		protected override void OnDeath()
		{
			_dropSpawner.Spawn(Center);
			Delete();
		}

		protected override void OnDamage(GameObject by, float amount)
		{
			var prop = _content.Particles.Get("Particles/blood.particle", Center);
			Game.Objects.WorldParticles.Emit(prop, 5);
		}
	}
}