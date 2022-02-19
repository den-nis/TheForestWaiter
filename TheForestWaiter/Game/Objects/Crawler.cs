using SFML.Graphics;
using SFML.System;
using System;
using System.Diagnostics;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Projectiles;

namespace TheForestWaiter.Game.Objects.Enemies
{
	internal class Crawler : GroundCreature
	{
		private const float CHASE_DISTANCE = 200; 
		private const float AVOID_DISTANCE = 100;
		private const float AVOID_DISTANCE_VARIATION = 20;

		private const float DEFAULT_WALK_SPEED = 120;
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

		public Crawler(GameData game, ContentSource content, DropSpawner dropSpawner, ObjectCreator creator) : base(game)
		{
			_content = content;
			_dropSpawner = dropSpawner;
			_creator = creator;
			_animation = content.Textures.CreateAnimatedSprite("Textures\\Enemies\\crawler.png");
			Size = _animation.Sheet.TileSize.ToVector2f();

			SetMaxHealth(50, true);
			JumpForce = 400;
			WalkSpeed = DEFAULT_WALK_SPEED;

			_avoidDistance = AVOID_DISTANCE + Rng.Variation(AVOID_DISTANCE_VARIATION);
			Debug.Assert(_avoidDistance < CHASE_DISTANCE, "Avoid distance should be less than chase distance");

			_attackTrigger = new RandomTrigger(PrepareAttack, 0.5f, 0.7f);

			_dropSpawner.Setup("Textures\\Enemies\\crawler_gibs.png");
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

			if (_prepareAttackTime > 0)
			{
				_animation.Framerate = (int)(5f / ATTACK_PREPARE_TIME);
				_animation.AnimationStart = 7;
				_animation.AnimationEnd = 12;
				_animation.Paused = false;
			}
			else
			{
				var isMovingRight = PlayerDirection > 1;
				var isMovingLeft = PlayerDirection < -1;

				if (isMovingLeft || isMovingRight)
				{
					_animation.Framerate = 20;
					_animation.AnimationStart = 14;
					_animation.AnimationEnd = 18;
					_animation.Paused = false;
				}
				else
				{
					_animation.Framerate = 7;
					_animation.AnimationStart = 0;
					_animation.AnimationEnd = 6;
					_animation.Paused = false;
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
			var bullet = _creator.FireBullet<CorruptionBall>(AttackPoint, velocity, this);
			Game.Objects.AddGameObject(bullet);
		}

		protected override void OnDeath()
		{
			_dropSpawner.Spawn(Center);
			Delete();
		}

		protected override void OnDamage(GameObject by)
		{
			var prop = _content.Particles.Get("Particles\\blood.particle", Center);
			Game.Objects.WorldParticles.Emit(prop, 5);
		}
	}
}