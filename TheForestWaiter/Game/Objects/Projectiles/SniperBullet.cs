using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	class SniperBullet : Bullet
    {
        public SniperBullet(GameData game, ContentSource content) : base(game, content)
        {
            Damage = 100;

            SetBulletSprite("Textures/Bullets/bullet_sniper.png");
        }

        public override void Update(float time)
        {
            base.Update(time);
        }
    }
}
