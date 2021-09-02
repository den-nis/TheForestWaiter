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
        public SniperBullet(GameData game, GameContent content, IGameDebug debug) : base(game, debug, content)
        {
            Damage = 100;

            Size = new Vector2f(5, 3);

            BulletSprite = content.Textures.CreateSprite("Textures\\Bullets\\bullet_sniper.png");
            BulletSprite.Origin = Size / 2;
            BulletSprite.Rotation = TrigHelper.ToDeg(StartAngle);
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
    }
}
