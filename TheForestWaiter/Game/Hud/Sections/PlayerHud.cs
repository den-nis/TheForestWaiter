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

		private readonly Sprite _boxSprite;
		private readonly Sprite _healthSprite;
		private readonly SpriteFont _coinText;
		private readonly GameData _game;

		public PlayerHud(float scale) : base(scale)
		{
			var content = IoC.GetInstance<ContentSource>();
			_game = IoC.GetInstance<GameData>();

			_boxSprite = content.Textures.CreateSprite("Textures/Hud/box.png");
			_healthSprite = content.Textures.CreateSprite("Textures/Hud/health.png");
			_coinText = new SpriteFont(content.Textures.CreateSpriteSheet("Textures/Hud/numbers.png"));

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
			_coinText.Position = GetPosition(window) + new Vector2f(20 * Scale, 15 * Scale);
			_coinText.Scale = Scale;
			_coinText.SetText(_game.Session.Coins.ToString());

			_coinText.Draw(window);
		}

		public override bool IsMouseOnAnyButton() => false;
		public override void OnMouseMove(Vector2i mouse) { }
		public override void OnPrimaryReleased() { }
		public override void OnPrimaryPressed() { }
	}
}
