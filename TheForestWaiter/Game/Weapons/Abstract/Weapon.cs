using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Game.Weapons.Abstract
{
	internal abstract class Weapon : Drawable
	{
		public float Weight { get; protected set; } = 0;

		public bool Firing { get; set; }
		public Color Color { get; set; }

		public abstract string IconTextureName { get; }

		public Creature Owner { get; set; }
		protected abstract Vector2f Origin { get; }
		protected abstract Vector2f AttachPoint { get; }

		protected bool AutoFire { get; set; } = true;
		protected float FireSpeed { get; set; } = 1000;
		protected float FireRatePerSecond { get; set; } = 10;
		protected float FireSpeedVariation { get; set; } = 0;
		protected float KickbackForce { get; set; } = 0;
		protected float Cone { get; set; } = 0;

		protected Vector2f OriginBarrelOffset => new(Sprite.Texture.Size.X - Origin.X, Origin.Y);
		protected Vector2f BarrelPosition => AttachPoint + TrigHelper.FromAngleRad(LastAimAngle, Sprite.Texture.Size.X - Origin.X);
		protected int AimingDirection { get; private set; } = 1;

		protected Sprite Sprite { get; set; }

		protected SoundInfo FireSound { get; set; }
		protected SoundInfo StuckSound { get; set; }

		public Vector2f LastAim { get; private set; }
		public float LastAimAngle { get; private set; }

		private readonly GameData _gameData;
		private readonly ObjectCreator _creator;
		private readonly SoundSystem _sound;
		private readonly NetContext _network;
		private float _fireTimer;
		private bool _firstShot;

		public Weapon()
		{
			_network = IoC.GetInstance<NetContext>();
			_gameData = IoC.GetInstance<GameData>();
			_creator = IoC.GetInstance<ObjectCreator>();
			_sound = IoC.GetInstance<SoundSystem>();
		}

		public abstract void OnFire(bool noProjectile);

		public void Aim(float angle)
		{
			AimingDirection = TrigHelper.IsPointingRight(angle) ? 1 : -1;
			LastAimAngle = angle;
		}

		public virtual void Draw(RenderWindow window)
		{
			window.Draw(Sprite);
		}

		public void Fire()
		{
			if (_gameData.World.TouchingSolid(BarrelPosition + TrigHelper.FromAngleRad(GetShotFromAngle(), 10)))
			{
				_sound.Play(StuckSound ?? SoundInfo.None);
			}
			else
			{
				_sound.Play(FireSound ?? SoundInfo.None);
				OnFire(_network.Settings.IsClient);
				Kickback();
			}
		}

		protected float GetShotFromAngle()
		{
			return LastAimAngle + (Cone * (Rng.Float() - 0.5f));
		}


		/// <summary>
		/// Gets called even when the weapon is not equiped (only if the player has the weapon available)
		/// </summary>
		public virtual void BackgroundUpdate(float time) { }

		public virtual void Update(float time)
		{
			Sprite.Color = Color;

			if (_fireTimer > 0)
				_fireTimer -= FireRatePerSecond * time;

			if (Firing)
			{
				while ((_firstShot || AutoFire) && _fireTimer <= 0)
				{
					_fireTimer += 1f;

					if (!_network.Settings.IsClient) 
					{
						Fire();
					}
				}
				_firstShot = false;
			}
			else
			{
				_firstShot = true;
			}

			Sprite.Scale = new Vector2f(1, AimingDirection);
			Sprite.Position = AttachPoint;
			Sprite.Origin = Origin;
			Sprite.Rotation = TrigHelper.ToDeg(LastAimAngle);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			target.Draw(Sprite);
		}

		private void Kickback()
		{
			Owner.Velocity += TrigHelper.FromAngleRad((float)(LastAimAngle - Math.PI), KickbackForce);
		}

		protected void FireProjectile<T>() where T : Projectile
		{
			var velocity = TrigHelper.FromAngleRad(GetShotFromAngle(), Rng.Var(FireSpeed, FireSpeedVariation));
			var bullet = _creator.FireProjectile<T>(BarrelPosition, velocity, Owner);
			_gameData.Objects.Projectiles.Add(bullet);

			if (_network.Settings.IsHost)
			{
				_network.Traffic.Send(new SpawnedProjectile
				{
					OwnerSharedId = Owner.SharedId,
					TypeIndex = Types.GetIndexByType(typeof(T)),
					Speed = velocity,
					Position = BarrelPosition,
				});
			}
		}
	}
}
