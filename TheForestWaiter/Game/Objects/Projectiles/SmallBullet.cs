using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class SmallBullet : Projectile
	{
		public SmallBullet(GameData game, ContentSource content, SoundSystem sound) : base(game, content, sound)
		{
			SetTexture("Textures/Projectiles/bullet_generic.png");
			Damage = 20;
		}
	}
}
