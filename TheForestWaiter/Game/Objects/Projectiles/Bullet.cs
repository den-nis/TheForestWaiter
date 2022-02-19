using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	class Bullet : Movable
	{
		public Creature Owner { get; set; }

		protected string ExplosionParticleName { get; set; } = "Particles/spark.particle";
		protected float Range { get; set; } = 1000;
		protected float Damage { get; set; } = 10;
		protected float Knockback { get; set; } = 150;

		private readonly ContentSource _content;
		private Vector2f? _spawn = null;
		private float? _startAngle = null;
		private float _traveled = 0;
		
		private Sprite _bulletSprite;

		public Bullet(GameData game, ContentSource content) : base(game)
		{
			_content = content;

			Size = new Vector2f(5, 5);
			Gravity = 0;

			SetBulletSprite("Textures/Bullets/bullet_generic.png");
		}

		protected void SetBulletSprite(string textureName)
		{
			_bulletSprite = _content.Textures.CreateSprite(textureName);
			_bulletSprite.Origin = _bulletSprite.Texture.Size.ToVector2f() / 2;
		}

		private void Explode()
		{
			Game.Objects.WorldParticles.Emit(_content.Particles.Get(ExplosionParticleName, Center), 10);
			Delete();
		}

		public override void Update(float time)
		{
			base.Update(time);

			_spawn ??= Center;
			_startAngle ??= Velocity.Angle();

			foreach (var creature in Game.Objects.Creatures)
			{
				if (Owner != null && creature.Friendly == Owner.Friendly)
				{
					continue;
				}

				if (Collisions.SweptAABB(creature.FloatRect, FloatRect, Velocity * time, out _) < 1 || Intersects(creature))
				{
					creature.Damage(this, Damage, Knockback); 
					Explode();
					return;
				}
			}

			_traveled = (Center - _spawn.Value).Len();
			if (_traveled > Range || CollisionFlags > 0)
			{
				Explode();
				return;
			}

			_bulletSprite.Rotation = TrigHelper.ToDeg(_startAngle.Value);
			_bulletSprite.Position = Center;
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_bulletSprite);
		}
	}
}
