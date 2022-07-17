using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Weapons
{
	internal class Chaingun : Weapon
	{
        public override string IconTextureName => "Textures/Weapons/Icons/chaingun.png";

        private readonly ContentSource _content;

        protected override Vector2f AttachPoint => Game.Objects.Player.Center - new Vector2f(0, 0);
        protected override Vector2f Origin => new(1, 5);

        private const int BEST_FIRERATE = 150;
        private const int OVER_HEAT = 10;
        private float _heat = 0;

        public Chaingun(GameData game, ContentSource content, ObjectCreator creator) : base(game, creator)
        {
            AutoFire = true;
            Cone = TrigHelper.ToRad(20);
            FireRatePerSecond = 100;

            Sprite = content.Textures.CreateSprite("Textures/Weapons/chaingun.png");
            _content = content;
        }

		public override void BackgroundUpdate(float time)
		{
			base.BackgroundUpdate(time);

            if (Firing)
            {
                _heat += time;
                _heat = Math.Min(OVER_HEAT, _heat);
            }
            else
            {
                _heat -= time;
                _heat = Math.Max(0, _heat);
            }
        }

        public override void Update(float time)
		{
            base.Update(time);

            var heat = _heat / OVER_HEAT;
            var color = heat * 200;

            FireRatePerSecond = BEST_FIRERATE - BEST_FIRERATE * heat;

            if (Sprite.Color == Color.White)
            {
                Sprite.Color = new Color(255, (byte)(255 - color), (byte)(255 - color));
            }
        }

        public override void OnFire()
        {
            Game.Objects.WorldParticles.Emit(_content.Particles.Get("Particles/handgun_smoke.particle", BarrelPosition, LastShotFromAngle, 120), 10);
            FireProjectile<ChainBullet>();
        }
    }
}
