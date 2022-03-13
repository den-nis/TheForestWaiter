using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class SmallBullet : Projectile
	{
		public SmallBullet(GameData game, ContentSource content) : base(game, content)
		{
			SetTexture("Textures/Projectiles/bullet_generic.png");
		}
	}
}
