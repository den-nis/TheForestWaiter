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
    internal class HudDrawer
    {
        private const int HEALTH_START_X = 16;
        private const int HEALTH_END_X = 50;

        public float HudScale { get; set; }

        private const int COIN_NUMBER_SPACING = 1;

        private readonly Sprite _boxSprite;
        private readonly Sprite _healthSprite;
        private readonly SpriteSheet _coinNumbers;

        private readonly GameData _game;
        private readonly Camera _camera;

        public HudDrawer(GameData game, Camera camera, ContentSource content, UserSettings settings)
        {
            _boxSprite = content.Textures.CreateSprite("Textures/Hud/box.png");
            _healthSprite = content.Textures.CreateSprite("Textures/Hud/health.png");
            _coinNumbers = content.Textures.CreateSpriteSheet("Textures/Hud/numbers.png");
       
            _game = game;
            _camera = camera;

            HudScale = settings.GetFloat("Game", "HudScale");
        }

        public void Update()
        {
            SetScale();
            SetPositions();

            float healthPercentage = _game.Objects.Player.Health / 100f;
            var healthRect = _healthSprite.TextureRect;
            healthRect.Width = (int)(HEALTH_START_X + ((HEALTH_END_X - HEALTH_START_X) * healthPercentage));
            _healthSprite.TextureRect = healthRect;
        }

        private void SetPositions()
        {
            _boxSprite.Position = new Vector2f(10, 10);
            _healthSprite.Position = new Vector2f(10, 10);
            _coinNumbers.Sprite.Position = new Vector2f(10 + 20 * HudScale, 10 + 15 * HudScale);
        }

        private void SetScale()
        {
            var scaleVector = new Vector2f(HudScale, HudScale);
            _boxSprite.Scale = scaleVector;
            _healthSprite.Scale = scaleVector;
            _coinNumbers.Sprite.Scale = scaleVector;
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(Camera.GetWindowView(window));

            window.Draw(_boxSprite);
            window.Draw(_healthSprite);

            DrawCoinText(window);

            window.SetView(_camera.GetView());
        }

        private void DrawCoinText(RenderWindow window)
        {
            string text = _game.Session.Coins.ToString();
            Vector2f sourcePosition = _coinNumbers.Sprite.Position;

            for (int i = 0; i < text.ToString().Length; i++)
            {
                Vector2f position = sourcePosition + new Vector2f((_coinNumbers.TileSize.X + COIN_NUMBER_SPACING) * i * HudScale, 0);
                var number = int.Parse(text[i].ToString());

                _coinNumbers.Sprite.Position = position;
                _coinNumbers.SetRect(number);

                window.Draw(_coinNumbers);
            }

            _coinNumbers.Sprite.Position = sourcePosition;
		}
    }
}
