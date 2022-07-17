using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class ChainBullet : Projectile
	{
		public ChainBullet(GameData game, ContentSource content) : base(game, content)
		{
			SetTexture("Textures/Projectiles/bullet_chain.png");
			ExplosionParticleName = "Particles/spark_gray.particle";
			Damage = 20;
		}
	}
}
