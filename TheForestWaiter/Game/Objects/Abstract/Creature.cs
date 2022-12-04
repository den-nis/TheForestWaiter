using System;
using System.Linq;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Game.Objects.Abstract
{
	/// <summary>
	/// Creatures can move around and be damaged
	/// </summary>
	internal abstract class Creature : Movable
	{
		public bool IsGhost { get; set; } = false;

		private const float PLAYER_SEARCH_INTERVAL = 1f;
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
		private Player _targetPlayerCache;
		private float _playerSearchTimer = 0;

		private readonly SoundSystem _sound;
		private readonly NetContext _net;

		public Creature()
		{
			_sound = IoC.GetInstance<SoundSystem>();
			_net = IoC.GetInstance<NetContext>();

			Health = _maxHealth;
		}

		public Player GetNearestPlayer()
		{
			if (_targetPlayerCache == null) CacheNearestPlayer();
			return _targetPlayerCache;
		}

		private void CacheNearestPlayer()
		{
			float nearest = float.PositiveInfinity;
			var target = _targetPlayerCache;
			foreach (var player in Game.Objects.Players.Where(p => p.Alive))
			{
				var delta = player.Position - Center;
				var distance = delta.Len();

				if (distance < nearest)
				{
					nearest = distance;
					target = player;
				}
			}
			_targetPlayerCache = target ?? Game.Objects.Players.First();
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

		public void Damage(Movable by, float amount, float knockback, bool fromServer = false)
		{
			if (_net.Settings.IsClient && !fromServer)
				return;
				
			if ((InvincibleWhenStunned && IsStunned) || !Alive)
				return;
			
			_stunTimer = StunTime;

			if (!Invincible)
			{
				_sound.Play(SoundOnDamage);
				Health -= amount;

				if (_net.Settings.IsHost)
				{
					_net.Traffic.Send(new ObjectDamaged
					{
						ForSharedId = SharedId,
						BySharedId = by.SharedId,
						Damage = amount,
						Knockback = knockback,
					});
				}

				if (Health <= 0)
				{
					Health = 0;
				}
			}

			OnDamage(by, amount);

			if (by != null && !IsGhost)
			{
				SetStunVelocity(by, knockback);
			}

			if (Health <= 0 && Alive)
			{
				Kill();
				return;
			}
		}

		public void Kill()
		{
			if (Net.Settings.IsHost)
			{
				Net.Traffic.Send(new ObjectKilled
				{
					SharedId = SharedId,
				});
			}

			Health = 0;
			Alive = false;
			OnDeath();
		}

		public override void Update(float time)
		{
			base.Update(time);

			_playerSearchTimer += time;
			if (_playerSearchTimer > PLAYER_SEARCH_INTERVAL)
			{
				CacheNearestPlayer();
			}

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
