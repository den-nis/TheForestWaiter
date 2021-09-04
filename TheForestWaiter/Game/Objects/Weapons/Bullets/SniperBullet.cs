using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Weapons.Bullets
{
    class SniperBullet : Bullet
    {
        public SniperBullet(GameData game, GameContent content) : base(game, content, "Textures\\Bullets\\bullet_sniper.png")
        {
            Damage = 100;
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
    }
}
