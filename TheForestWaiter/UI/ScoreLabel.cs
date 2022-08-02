using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Graphics;
using TheForestWaiter.UI.Abstract;

namespace TheForestWaiter.UI
{
	internal class ScoreLabel : Control
	{
		private const float SPACING = 20f;

		private readonly PlayStats _session;
		private readonly SpriteFont _numbersSprite;
		private readonly Sprite _textSprite;

		public ScoreLabel()
		{
			var content = IoC.GetInstance<ContentSource>();
			_session = IoC.GetInstance<PlayStats>();

			_numbersSprite = new SpriteFont(content.Textures.CreateSpriteSheet("Textures/Menu/score_numbers.png"));
			_textSprite = content.Textures.CreateSprite("Textures/Menu/wave.png");
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_textSprite);
			window.Draw(_numbersSprite);
		}

		public override void Update(float time)
		{
			_textSprite.Position = ActualPosition;
			_numbersSprite.Position = ActualPosition + new Vector2f(_textSprite.Texture.Size.X + SPACING, 0);
			_numbersSprite.SetText(_session.Wave.ToString());
		}
	}
}
