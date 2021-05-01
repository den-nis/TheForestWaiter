using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Objects.Weapons.Bullets;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Objects.Weapons.Guns
{ 
    class Sniper : GunBase
    {
        private float _smokeTimer = 0;
        private float _smokeEmitTimer = 0;
        private const float SMOKE_TIME_BETWEEN_EMIT = 0.005f;
        private const float SMOKE_TIME = 0.2f;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override float Range => 5000;
        protected override float FireRatePerSecond => 1f;
        protected override bool AutoFire => false;
        protected override Vector2f Origin => new(0f, 3f);
        protected override float FireSpeed => 2000;
        protected override float FireSpeedVariation => 0;
        protected override float Cone => 0;

        public Sniper(GameData game) : base(game, GameContent.Textures.CreateSprite("Textures\\Guns\\sniper.png"))
        {
            OnFire += OnFireEvent;
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
                    var smoke = GameContent.Particles.Get("Particles\\sniper_smoke.particle", BarrelPosition, TrigHelper.Up, 10);
                    Game.Objects.WorldParticles.Emit(smoke, 1);
                    _smokeEmitTimer -= SMOKE_TIME_BETWEEN_EMIT;
                }

                _smokeEmitTimer += time;
                _smokeTimer -= time;

            }

            base.Update(time);
        }

		protected override void FireBullet()
		{
            var bullet = new SniperBullet(Game, BarrelPosition, TrigHelper.FromAngleRad(LastShotFromAngle, FireSpeed + (FireSpeedVariation * (Rng.Float() - 0.5f))), Range);
            Game.Objects.Bullets.Add(bullet);
        }

		private void OnFireEvent()
        {
            Game.Objects.Player.velocity += TrigHelper.FromAngleRad((float)(LastAimAngle - Math.PI), 100);
            _smokeTimer = SMOKE_TIME;
            var prop = GameContent.Particles.Get("Particles\\sniper_muzzle_flash.particle", BarrelPosition, LastAimAngle, 320);
            Game.Objects.WorldParticles.Emit(prop, 20);
        }
    }
}
