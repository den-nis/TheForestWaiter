using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Abstract
{
	internal abstract class Projectile : Movable
	{
		public Creature Owner { get; private set; }

		protected string ExplosionParticleName { get; set; } = "Particles/spark.particle";
		protected string TrailParticleName { get; set; }
		protected bool EnableTrail { get; set; } = false;
		protected bool RemoveOnLowMovement { get; set; } = true; //If a bullet some how stops moving, remove it.

		protected float RotationSpeed { get; set; } = 100;
		protected float Range { get; set; } = 2000;
		protected float Damage { get; set; } = 5;
		protected float Knockback { get; set; } = 150;
		protected int Penetration { get; set; } = 1;

		private readonly SoundInfo _impactSound = new("Sounds/wall_hit.wav");
		private readonly List<long> _damangedCreatureIds = new();
		private readonly ContentSource _content;
		private readonly SoundSystem _sound;
		private Vector2f? _spawn = null;
		private float? _angle;
		private float _traveled = 0;
		private int _penetrated = 0;

		private Sprite _sprite;

		public Projectile()
		{
			_impactSound.Volume = 70;
			_content = IoC.GetInstance<ContentSource>();
			_sound = IoC.GetInstance<SoundSystem>();
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
			_sprite = _content.Textures.CreateSprite(textureName);
			_sprite.Origin = _sprite.Texture.Size.ToVector2f() / 2;
			Size = _sprite.Texture.Size.ToVector2f();
		}

		private void Explode()
		{
			if (ExplosionParticleName != null)
			{
				Game.Objects.WorldParticles.Emit(_content.Particles.Get(ExplosionParticleName, Center), 10);
			}

			Delete();
		}

		public override void Update(float time)
		{
			base.Update(time);


			if (RemoveOnLowMovement && Velocity.Len() < 0.1f)
			{
				Explode();
				return;
			}

			_spawn ??= Center;
			_angle ??= Velocity.Angle();

			float currentAngle = _angle.Value;
			float desiredAngle = Velocity.Angle();

			var delta = (float)Math.Atan2(Math.Sin(desiredAngle - currentAngle), Math.Cos(desiredAngle - currentAngle));
			_angle += delta * time * RotationSpeed * (float)Math.PI;

			foreach (var creature in Game.Objects.Creatures)
			{
				if (Owner != null && creature.Friendly == Owner.Friendly)
				{
					continue;
				}

				bool touching = Collisions.SweptAABB(creature.FloatRect, FloatRect, Velocity * time, out _) < 1 || Intersects(creature);

				if (touching && !_damangedCreatureIds.Contains(creature.GameObjectId))
				{
					creature.Damage(this, Damage, Knockback);
					_damangedCreatureIds.Add(creature.GameObjectId);
					_penetrated++;

					if (_penetrated >= Penetration)
					{
						Explode();
						return;
					}
				}
			}

			_traveled = (Center - _spawn.Value).Len();
			if (_traveled > Range || CollisionFlags > 0)
			{
				if (CollisionFlags > 0)
					_sound.Play(_impactSound);

				Explode();
				return;
			}

			_sprite.Rotation = TrigHelper.ToDeg(_angle.Value);
			_sprite.Position = Center;
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_sprite);
		}

		public override void OnMarkedForDeletion()
		{
			_sprite.Dispose();
		}
	}
}
