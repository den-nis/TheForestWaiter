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

        protected override Vector2f AttachPoint => Game.Objects.Player.Center;
        protected override Vector2f Origin => new(0f, 3f);

		public Shotgun(GameData game, ContentSource content, ObjectCreator creator) : base(game, creator)
        { 
            AutoFire = false;
            Cone = TrigHelper.ToRad(12);
            FireRatePerSecond = 1;
            FireSpeedVariation = 300;
            FireSpeed = 1500;
            KickbackForce = 50;

            Sprite = content.Textures.CreateSprite("Textures/Weapons/shotgun.png");
            FireSound = content.Sounds.CreateGameSound("Sounds/Weapons/shotgun_{n}.wav");
            _content = content;
        }

		public override void OnFire()
		{
            Game.Objects.WorldParticles.Emit(_content.Particles.Get("Particles/handgun_smoke.particle", BarrelPosition, GetShotFromAngle(), 120), 10);

            for (int i = 0; i < 10; i++)
            {
                FireProjectile<ShotgunBullet>();
            }
		}
	}
}
