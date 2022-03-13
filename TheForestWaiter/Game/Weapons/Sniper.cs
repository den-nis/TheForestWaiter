using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Objects.Weapons.Abstract;

namespace TheForestWaiter.Game.Objects.Weapons
{
	internal class Sniper : Weapon
    {
        public override string IconTextureName => "Textures/Weapons/Icons/sniper.png";

        private float _smokeTimer = 0;
        private float _smokeEmitTimer = 0;
        private const float SMOKE_TIME_BETWEEN_EMIT = 0.005f;
        private const float SMOKE_TIME = 0.2f;

        private readonly ContentSource _content;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override Vector2f Origin => new(0f, 3f);

		public Sniper(GameData game, ContentSource content, ObjectCreator creator) : base(game, creator)
        {
            _content = content;

            Cone = 0;
            FireSpeed = 2000;
            FireRatePerSecond = 1;
            KickbackForce = 120;
            AutoFire = false;

            Sprite = content.Textures.CreateSprite("Textures/Weapons/sniper.png");
        }

        public override void Draw(RenderWindow win)
        {
            base.Draw(win);
        }

        public override void Update(float time)
        {
            if (_smokeTimer > 0)
            {
                while (_smokeEmitTimer > SMOKE_TIME_BETWEEN_EMIT)
                {
                    var smoke = _content.Particles.Get("Particles/sniper_smoke.particle", BarrelPosition, TrigHelper.Up, 10);
                    Game.Objects.WorldParticles.Emit(smoke, 1);
                    _smokeEmitTimer -= SMOKE_TIME_BETWEEN_EMIT;
                }

                _smokeEmitTimer += time;
                _smokeTimer -= time;

            }

            base.Update(time);
        }

		public override void OnFire()
		{
            _smokeTimer = SMOKE_TIME;

            var prop = _content.Particles.Get("Particles/sniper_muzzle_flash.particle", BarrelPosition, LastAimAngle, 320);
            Game.Objects.WorldParticles.Emit(prop, 20);

            FireProjectile<SniperBullet>();
        }
	}
}
