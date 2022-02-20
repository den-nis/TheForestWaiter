using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class PlayerHud : HudSection
	{
		private const int HEALTH_START_X = 16;
		private const int HEALTH_END_X = 50;
		private const int COIN_NUMBER_SPACING = 1;

		private readonly Sprite _boxSprite;
		private readonly Sprite _healthSprite;
		private readonly SpriteSheet _coinNumbers;
		private readonly GameData _game;

		public PlayerHud(GameData game, ContentSource content)
		{
			_game = game;
			_boxSprite = content.Textures.CreateSprite("Textures/Hud/box.png");
			_healthSprite = content.Textures.CreateSprite("Textures/Hud/health.png");
			_coinNumbers = content.Textures.CreateSpriteSheet("Textures/Hud/numbers.png");

			Size = _boxSprite.Texture.Size.ToVector2f();
		}

		public override void Draw(RenderWindow window)
		{
			DrawBox(window);
			DrawCoinText(window);
			DrawHealth(window);
		}

		private void DrawHealth(RenderWindow window)
		{
			float healthPercentage = _game.Objects.Player.Health / 100f;
			var healthRect = _healthSprite.TextureRect;
			healthRect.Width = (int)(HEALTH_START_X + ((HEALTH_END_X - HEALTH_START_X) * healthPercentage));
			_healthSprite.TextureRect = healthRect;
			_healthSprite.Position = GetPosition(window);
			_healthSprite.Scale = ScaleVector;

			window.Draw(_healthSprite);
		}

		private void DrawBox(RenderWindow window)
		{
			_boxSprite.Position = GetPosition(window);
			_boxSprite.Scale = ScaleVector;

			window.Draw(_boxSprite);
		}

		private void DrawCoinText(RenderWindow window)
		{
			_coinNumbers.Sprite.Scale = ScaleVector;

			var sectionPosition = GetPosition(window) + new Vector2f(20 * Scale, 15 * Scale);
			string text = _game.Session.Coins.ToString();

			for (int i = 0; i < text.ToString().Length; i++)
			{
				Vector2f position = sectionPosition + new Vector2f((_coinNumbers.TileSize.X + COIN_NUMBER_SPACING) * i * Scale, 0);
				var number = int.Parse(text[i].ToString());

				_coinNumbers.Sprite.Position = position;
				_coinNumbers.SetRect(number);

				window.Draw(_coinNumbers);
			}
		}
	}
}
