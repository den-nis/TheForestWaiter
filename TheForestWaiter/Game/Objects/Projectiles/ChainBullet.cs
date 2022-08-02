using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class ChainBullet : Projectile
	{
		public ChainBullet()
		{
			SetTexture("Textures/Projectiles/bullet_chain.png");
			ExplosionParticleName = "Particles/spark_gray.particle";
			Damage = 10;
			Range = 1200;
		}
	}
}
