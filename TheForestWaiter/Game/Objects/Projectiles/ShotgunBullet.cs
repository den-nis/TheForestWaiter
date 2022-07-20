using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class ShotgunBullet : Projectile
	{
		public ShotgunBullet(GameData game, ContentSource content, SoundSystem sound) : base(game, content, sound)
		{
			SetTexture("Textures/Projectiles/bullet_shotgun.png");
			ExplosionParticleName = "Particles/spark_gray.particle";
			Damage = 35;
		}
	}
}
