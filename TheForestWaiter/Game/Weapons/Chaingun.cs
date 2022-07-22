using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Weapons
{
	internal class Chaingun : Weapon
	{
		public override string IconTextureName => "Textures/Weapons/Icons/chaingun.png";

		private readonly ContentSource _content;

		protected override Vector2f AttachPoint => _gameData.Objects.Player.Center - new Vector2f(0, 0);
		protected override Vector2f Origin => new(1, 5);

		private const int BEST_FIRERATE = 150;
		private const int OVER_HEAT = 13;
		private const int TIME_STUCK = 3;
		private float _stuckTimer = 0;
		private float _heat = 0;

		private readonly GameData _gameData;

		public Chaingun()
		{
			_gameData = IoC.GetInstance<GameData>();
			_content = IoC.GetInstance<ContentSource>();

			AutoFire = true;
			Cone = TrigHelper.ToRad(20);
			FireRatePerSecond = 80;
			FireSpeedVariation = 100;
			Weight = 0.4f;

			Sprite = _content.Textures.CreateSprite("Textures/Weapons/chaingun.png");
			FireSound = new("Sounds/Weapons/chaingun.wav")
			{
				PitchVariation = 0.3f,
				Volume = 20f
			};
		}

		public override void BackgroundUpdate(float time)
		{
			base.BackgroundUpdate(time);

			if (_stuckTimer > 0)
			{
				_stuckTimer -= time;
				return;
			}

			if (Firing)
			{
				_heat += time;

				if (_heat > OVER_HEAT)
				{
					_stuckTimer = TIME_STUCK;
					return;
				}
			}
			else
			{
				_heat -= time;
				_heat = Math.Max(0, _heat);
			}
		}

		public override void Update(float time)
		{
			base.Update(time);

			var heat = _heat / OVER_HEAT;
			var color = heat * 200;

			FireRatePerSecond = BEST_FIRERATE - BEST_FIRERATE * heat;

			if (Sprite.Color == Color.White)
			{
				Sprite.Color = new Color(255, (byte)(255 - color), (byte)(255 - color));
			}
		}

		public override void OnFire()
		{
			_gameData.Objects.WorldParticles.Emit(_content.Particles.Get("Particles/handgun_smoke.particle", BarrelPosition, GetShotFromAngle(), 400), 10);
			FireProjectile<ChainBullet>();
		}
	}
}
