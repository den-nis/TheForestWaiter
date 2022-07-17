using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Weapons
{
	internal class Bow : Weapon
    {
		public override string IconTextureName => "Textures/Weapons/Icons/bow.png";

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 1);
        protected override Vector2f Origin => new(2.5f, 8.5f);

		public Bow(GameData game, ContentSource content, ObjectCreator creator) : base(game, creator)
        { 
            AutoFire = false;
            Cone = TrigHelper.ToRad(5);
            FireRatePerSecond = 3;
            FireSpeedVariation = 10;
            FireSpeed = 500;

            Sprite = content.Textures.CreateSprite("Textures/Weapons/bow.png");
        }

		public override void OnFire()
		{
            FireProjectile<Arrow>();
		}
	}
}
