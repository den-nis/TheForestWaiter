using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Hud
{
    class HudDrawer
    {
        private const int HEALTH_START_X = 16;
        private const int HEALTH_END_X = 50;

        private const int AMMO_START_X = 16;
        private const int AMMO_END_X = 51;

        private readonly Sprite _ammoSprite;
        private readonly Sprite _boxSprite;
        private readonly Sprite _healthSprite;

        private readonly GameData _game;
        private readonly Camera _camera;

        public HudDrawer(GameData game, Camera camera, ContentSource content, UserSettings settings)
        {
            _ammoSprite = content.Textures.CreateSprite("Textures\\Hud\\ammo.png");
            _boxSprite = content.Textures.CreateSprite("Textures\\Hud\\box.png");
            _healthSprite = content.Textures.CreateSprite("Textures\\Hud\\health.png");

            _game = game;
            _camera = camera;

            _boxSprite.Position = new Vector2f(10, 10);
            _healthSprite.Position = new Vector2f(10, 10);
            _ammoSprite.Position = new Vector2f(10, 10);

            var scale = settings.GetFloat("Game", "HudScale");
            _boxSprite.Scale = new Vector2f(scale, scale);
            _healthSprite.Scale = new Vector2f(scale, scale); 
            _ammoSprite.Scale = new Vector2f(scale, scale); 
        }

        public void Update()
        {
            float healthPercentage = _game.Objects.Player.Health / 100f;
            var healthRect = _healthSprite.TextureRect;
            healthRect.Width = (int)(HEALTH_START_X + ((HEALTH_END_X - HEALTH_START_X) * healthPercentage));
            _healthSprite.TextureRect = healthRect;

            float ammoPercentage = 0;
            var gun = _game.Objects.Player.Gun;
            if (gun != null)
            {
                ammoPercentage = (float)gun.Ammo / gun.MaxAmmo;
            }

            var ammoRect = _healthSprite.TextureRect;
            ammoRect.Width = (int)(AMMO_START_X + ((AMMO_END_X - AMMO_START_X) * ammoPercentage));
            _ammoSprite.TextureRect = ammoRect;
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(Camera.GetWindowView(window));

            window.Draw(_boxSprite);
            window.Draw(_ammoSprite);
            window.Draw(_healthSprite);

            window.SetView(_camera.GetView());
        }
    }
}
