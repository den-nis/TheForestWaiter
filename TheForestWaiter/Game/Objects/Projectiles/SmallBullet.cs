﻿using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class SmallBullet : Projectile
	{
		public SmallBullet()
		{
			SetTexture("Textures/Projectiles/bullet_generic.png");
			Damage = 20;
		}
	}
}
