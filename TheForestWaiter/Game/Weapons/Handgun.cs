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
        private readonly ContentSource _content;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override Vector2f Origin => new(2.5f, 6.5f);

        public Handgun(GameData game, ContentSource content, ObjectCreator creator) : base(game, creator)
        { 
            AutoFire = true;
            FireRatePerSecond = 100;
            Cone = 1;
            GunSprite = content.Textures.CreateSprite("Textures\\Guns\\handgun.png");
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
