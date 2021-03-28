using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Entities;
using TheForestWaiter.Essentials;
using TheForestWaiter.Particles;
using TheForestWaiter.Particles.Templates;

namespace TheForestWaiter.Objects.Weapons.Bullets
{
    class SniperBullet : Bullet
    {
        public SniperBullet(GameData game, Vector2f spawn, Vector2f speed, float range) : base(game, spawn, speed, range)
        {
            Damage = 100;

            Size = new Vector2f(5, 3);

            BulletSprite = GameContent.Textures.CreateSprite("Textures\\Bullets\\bullet_sniper.png");
            BulletSprite.Origin = Size / 2;
            BulletSprite.Rotation = TrigHelper.ToDeg(StartAngle);
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
    }
}
