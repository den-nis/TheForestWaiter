using System;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Abstract
{
	/// <summary>
	/// Creatures can move around and be damaged
	/// </summary>
	internal abstract class Creature : Movable
	{
		private const float REMOVE_AT_Y = 6000;
		private const float KNOCKBACK_VARIATION = 0.10f;
		private const float MOVING_DIRECTION_THRESHOLD = 0.1f;
		private const float STUN_CONE = (float)Math.PI / 16;

		public float Health { get; private set; }
		public float HealthPercentage => _maxHealth / Health;
		public bool Alive { get; private set; } = true;
		public bool IsStunned => _stunTimer > 0;
		public bool InvincibleWhenStunned { get; set; } = false;

		public bool Friendly { get; protected set; } = false;
		protected bool Invincible { get; set; }
		protected float StunTime { get; set; } = 0.1f;
		protected float KnockbackResistance { get; set; } = 0;
		protected SoundInfo SoundOnDamage = new("Sounds/Enemies/small_hurt.wav");

		/// <summary>
		/// Direction creature is facing based on the last move direction
		/// </summary>
		protected int FacingDirection { get; private set; } = 1;
		protected int MovingDirection { get; private set; } = 0;

		private float _maxHealth = 100;
		private float _stunTimer = 0;

		private readonly SoundSystem _sound;

		public Creature()
		{
			_sound = IoC.GetInstance<SoundSystem>();

			Health = _maxHealth;
		}

		protected void SetMaxHealth(int amount, bool fill)
		{
			_maxHealth = amount;
			Health = Math.Min(Health, _maxHealth);

			if (fill)
				Health = _maxHealth;
		}

		public void Heal(float amount)
		{
			Health += amount;
			Health = Math.Min(_maxHealth, Health);
		}

		public void Damage(Movable by, float amount, float knockback)
		{
			if ((InvincibleWhenStunned && IsStunned) || !Alive)
			{
				return;
			}

			_stunTimer = StunTime;

			if (!Invincible)
			{
				_sound.Play(SoundOnDamage);
				Health -= amount;
				
				if (Health <= 0)
				{
					Health = 0;
				}
			}

			if (Health <= 0 && Alive)
			{
				Alive = false;
				OnDeath();
			}

			if (by != null)
			{
				SetStunVelocity(by, knockback);
			}

			OnDamage(by, amount);
		}

		public override void Update(float time)
		{
			base.Update(time);

			if (_stunTimer > 0)
			{
				_stunTimer -= time;
			}

			var speed = GetSpeed();
			if (speed.X > MOVING_DIRECTION_THRESHOLD)
			{
				FacingDirection = 1;
			}

			if (speed.X < -MOVING_DIRECTION_THRESHOLD)
			{
				FacingDirection = -1;
			}

			if (Math.Abs(speed.X) > MOVING_DIRECTION_THRESHOLD)
			{
				MovingDirection = Math.Sign(speed.X);
			}
			else
			{
				MovingDirection = 0;
			}

			if (Position.Y > REMOVE_AT_Y)
			{
				Delete();
			}
		}

		protected abstract void OnDeath();

		protected abstract void OnDamage(GameObject by, float amount);

		private void SetStunVelocity(Movable by, float knockback)
		{
			var variation = Rng.Range(-KNOCKBACK_VARIATION, KNOCKBACK_VARIATION) * knockback;
			var knockbackForce = Math.Max(0, (knockback + variation) - KnockbackResistance);

			if (knockbackForce > 0)
			{
				var angle = (Center - by.Center).Angle();
				angle += Rng.Range(-STUN_CONE, STUN_CONE);
				Velocity = TrigHelper.FromAngleRad(angle, knockbackForce);
			}
		}
	}
}
