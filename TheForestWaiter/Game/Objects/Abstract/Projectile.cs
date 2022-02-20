using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Abstract
{
	internal abstract class Projectile : Movable
	{
		public Creature Owner { get; private set; }

		protected string ExplosionParticleName { get; set; } = "Particles/spark.particle";
		protected string TrailParticleName { get; set; }
		protected bool EnableTrail { get; set; } = false;

		protected int RotationSmoothness { get; set; } = 10;
		protected float Range { get; set; } = 1000;
		protected float Damage { get; set; } = 5;
		protected float Knockback { get; set; } = 150;
		protected int Penetration { get; set; } = 1;

		private readonly ContentSource _content;
		private Vector2f? _spawn = null;
		private float? _angle;
		private float _traveled = 0;
		private int _penetrated = 0;

		private Sprite _bulletSprite;

		public Projectile(GameData game, ContentSource content) : base(game)
		{
			_content = content;
			Gravity = 0;
		}

		public void Claim(Creature owner)
		{
			if (Owner != null)
				throw new InvalidOperationException("Owner is already set");

			Owner = owner;
		}

		protected void SetTexture(string textureName)
		{
			_bulletSprite = _content.Textures.CreateSprite(textureName);
			_bulletSprite.Origin = _bulletSprite.Texture.Size.ToVector2f() / 2;
			Size = _bulletSprite.Texture.Size.ToVector2f();
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
			_angle ??= Velocity.Angle();
			_angle = (Velocity.Angle() * RotationSmoothness) / RotationSmoothness;

			foreach (var creature in Game.Objects.Creatures)
			{
				if (Owner != null && creature.Friendly == Owner.Friendly)
				{
					continue;
				}

				if (Collisions.SweptAABB(creature.FloatRect, FloatRect, Velocity * time, out _) < 1 || Intersects(creature))
				{
					creature.Damage(this, Damage, Knockback);
					_penetrated++;

					if (_penetrated >= Penetration)
					{
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
			}

			_bulletSprite.Rotation = TrigHelper.ToDeg(_angle.Value);
			_bulletSprite.Position = Center;
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_bulletSprite);
		}
	}
}
