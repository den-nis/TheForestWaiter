using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Objects.Weapons.Guns
{ 
    class Handgun : GunBase
    {
        private readonly GameContent _content;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override float Range => 1500;
        protected override float FireRatePerSecond => 3f;
        protected override bool AutoFire => false;
        protected override Vector2f Origin => new(2.5f, 6.5f);
        protected override float FireSpeed => 500;
        protected override float FireSpeedVariation => 100;
        protected override float Cone => 0;

        public Handgun(GameData game, GameContent content, Camera camera, ObjectCreator creator) : base(game, camera, creator)
        {
            OnFire += OnFireEvent;
            _content = content;
        }

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
            Game.Objects.WorldParticles.Emit(_content.Particles.Get("Particles\\handgun_smoke.particle", BarrelPosition, LastShotFromAngle, 120), 10);
        }
    }
}
