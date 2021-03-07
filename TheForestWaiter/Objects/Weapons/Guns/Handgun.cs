using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Particles;
using TheForestWaiter.Particles.Templates;

namespace TheForestWaiter.Objects.Weapons.Guns
{ 
    class Handgun : GunBase
    {
        public Handgun(GameData game) : base(game, GameContent.Textures.CreateSprite("Textures\\Player\\small_gun.png"))
        {
            OnFire += OnFireEvent;
        }

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override float Range => 1500;
        protected override float FireRatePerSecond => 3f;
        protected override bool AutoFire => false;
        protected override Vector2f Origin => new Vector2f(2.5f,6.5f);
        protected override float FireSpeed => 500;
        protected override float FireSpeedVariation => 100;
        protected override float Cone => 0;

        public override void Draw(RenderWindow win)
        {
            base.Draw(win);
        }

        public override void Update(float time)
        {
            base.Update(time);
        }

        private void OnFireEvent()
        {
            Game.Objects.WorldParticles.Emit(ParticleTemplates.Spark, 10);
        }
    }
}
