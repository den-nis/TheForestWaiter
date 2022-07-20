using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class ChainBullet : Projectile
	{
		public ChainBullet(GameData game, ContentSource content, SoundSystem sound) : base(game, content, sound)
		{
			SetTexture("Textures/Projectiles/bullet_chain.png");
			ExplosionParticleName = "Particles/spark_gray.particle";
			Damage = 20;
		}
	}
}
