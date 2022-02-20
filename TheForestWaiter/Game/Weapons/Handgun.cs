using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Objects.Weapons.Abstract;

namespace TheForestWaiter.Game.Objects.Weapons.Guns
{
	internal class Handgun : ProjectileLauncher
    {
        private readonly ContentSource _content;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override Vector2f Origin => new(2.5f, 6.5f);

        public Handgun(GameData game, ContentSource content, ObjectCreator creator) : base(game, creator)
        { 
            AutoFire = true;
            Cone = TrigHelper.ToRad(5);
            FireRatePerSecond = 3;

            Sprite = content.Textures.CreateSprite("Textures/Guns/handgun.png");
            _content = content;
        }

		public override void OnFire()
		{
            Game.Objects.WorldParticles.Emit(_content.Particles.Get("Particles/handgun_smoke.particle", BarrelPosition, LastShotFromAngle, 120), 10);
            FireProjectile<SmallBullet>();
		}
	}
}
