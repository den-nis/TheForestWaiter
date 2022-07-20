using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Weapons
{
	internal class Handgun : Weapon
    {
		public override string IconTextureName => "Textures/Weapons/Icons/handgun.png";

        private readonly ContentSource _content;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override Vector2f Origin => new(2.5f, 6.5f);

		public Handgun(GameData game, ContentSource content, ObjectCreator creator, SoundSystem sound) : base(game, creator, sound)
        { 
            AutoFire = true;
            Cone = TrigHelper.ToRad(5);
            FireRatePerSecond = 5;

            Sprite = content.Textures.CreateSprite("Textures/Weapons/handgun.png");
            FireSound = new("Sounds/Weapons/handgun_{n}.wav");

            _content = content;
        }

		public override void OnFire()
		{
            Game.Objects.WorldParticles.Emit(_content.Particles.Get("Particles/handgun_smoke.particle", BarrelPosition, GetShotFromAngle(), 120), 10);
            FireProjectile<SmallBullet>();
		}
	}
}
