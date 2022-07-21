using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Weapons
{
	internal class Shotgun : Weapon
    {
		public override string IconTextureName => "Textures/Weapons/Icons/shotgun.png";

        private readonly ContentSource _content;

        protected override Vector2f AttachPoint => _gameData.Objects.Player.Center;
        protected override Vector2f Origin => new(0f, 3f);

        private readonly GameData _gameData;

		public Shotgun()
        {
            _content = IoC.GetInstance<ContentSource>();
            _gameData = IoC.GetInstance<GameData>();

            AutoFire = false;
            Cone = TrigHelper.ToRad(12);
            FireRatePerSecond = 1;
            FireSpeedVariation = 300;
            FireSpeed = 1500;
            KickbackForce = 50;

            Sprite = _content.Textures.CreateSprite("Textures/Weapons/shotgun.png");
            FireSound = new("Sounds/Weapons/shotgun_{n}.wav");
        }

		public override void OnFire()
		{
            _gameData.Objects.WorldParticles.Emit(_content.Particles.Get("Particles/handgun_smoke.particle", BarrelPosition, GetShotFromAngle(), 120), 10);

            for (int i = 0; i < 10; i++)
            {
                FireProjectile<ShotgunBullet>();
            }
		}
	}
}
