﻿using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class SniperBullet : Projectile
    {
        public SniperBullet()
        {
            Damage = 100;
            Penetration = 7;
            SetTexture("Textures/Projectiles/bullet_sniper.png");
        }
    }
}
