using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Objects.Weapons.Guns
{ 
    class SmallGun : GunBase
    {
        ParticleSystem Particles { get; set; } = new ParticleSystem(1000);

        public SmallGun(GameData game) : base(game, GameContent.Textures.CreateSprite("Textures\\Player\\small_gun.png"))
        {
            OnFire += OnFireEvent;
        }

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override float Range => 1500;
        protected override float FireRatePerSecond => 12f;
        protected override float RecoilPerShot => 0;
        protected override float Damage => 214;
        protected override bool AutoFire => true;
        protected override Vector2f Origin => new Vector2f(2.5f,6.5f);
        protected override float FireSpeed => 500;
        protected override float FireSpeedVariation => 100;
        protected override float Cone => (float)Math.PI / 10;

        public override void Draw(RenderWindow win)
        {
            Particles.Draw(win);
            base.Draw(win);
        }

        public override void Update(float time)
        {
            Particles.Update(time);
            base.Update(time);
        }

        private void OnFireEvent()
        {
            Game.Objects.Player.velocity += TrigHelper.FromAngleRad(LastAimAngle + (float)Math.PI, RecoilPerShot);

            Particles.Emit(new ParticleProp
            {
                ColorEnd = new Color(20, 20, 20, 0),
                ColorStart = new Color(200, 200, 200),
                RotationSpeedVariation = 20,
                SizeStart = 2,
                SizeEnd = 4,
                Velocity = TrigHelper.FromAngleRad(LastAimAngle, 400) + new Vector2f(Game.Objects.Player.RealSpeed.X, 0),
                VelocityVariation = new Vector2f(200, 200),
                Life = .1f,
                LifeVariation = .1f,
                Position = BarrelPosition,
            }, 10);
        }
    }
}
