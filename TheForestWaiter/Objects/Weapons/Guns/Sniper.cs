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
using TheForestWaiter.Particles.Templates;

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
        protected override float FireRatePerSecond => 0.6f;
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
                    var smoke = ParticleTemplates.SniperSmoke;
                    smoke.Position = BarrelPosition;
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
            _smokeTimer = SMOKE_TIME;

            Game.Objects.WorldParticles.Emit(new ParticleProp
            {
                ColorEnd = new Color(20, 20, 20, 0),
                ColorStart = new Color(200, 200, 200),
                RotationSpeedVariation = 20,
                SizeStart = 2,
                SizeEnd = 4,
                Velocity = TrigHelper.FromAngleRad(LastAimAngle, 100) + new Vector2f(Game.Objects.Player.RealSpeed.X, 0),
                VelocityVariation = new Vector2f(200, 200),
                Life = .1f,
                LifeVariation = .1f,
                Position = BarrelPosition,
            }, 30);

            Game.Objects.WorldParticles.Emit(new ParticleProp
            {
                ColorEnd = new Color(255, 216, 0, 0),
                ColorStart = new Color(200, 200, 200),
                RotationSpeedVariation = 20,
                SizeStart = 2,
                SizeEnd = 4,
                Velocity = TrigHelper.FromAngleRad(LastAimAngle, 200) + new Vector2f(Game.Objects.Player.RealSpeed.X, 0),
                VelocityVariation = new Vector2f(50, 50),
                Life = .8f,
                LifeVariation = .1f,
                Position = BarrelPosition,
            }, 10);
        }
    }
}
