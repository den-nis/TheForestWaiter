using SFML.Graphics;
using SFML.System;

namespace TheForestWaiter.Graphics
{
	internal class SpriteFont : Drawable
	{
		public Vector2f Position { get; set; }
		public Vector2f Spacing { get; set; } = new Vector2f(1, 1);
		public Color Color { get; set; } = Color.White;

		public float Scale { get; set; } = 1;
		public int IndexOffset { get; set; } = '0';

		private readonly SpriteSheet _sheet;
		private string _text = string.Empty;

		public SpriteFont(SpriteSheet sheet)
		{
			_sheet = sheet;
		}

		public void SetText(string text)
		{
			_text = text;
		}

		public void Draw(RenderTarget window)
		{
			float offsetX = 0;
			float offsetY = 0;

			_sheet.Sprite.Color = Color;
			for (int i = 0; i < _text.Length; i++)
			{
				char character = _text[i];

				if (character == '\n')
				{
					offsetY += (_sheet.TileSize.Y + Spacing.Y) * Scale;
					offsetX = 0;
					continue;
				}

				int index = GetIndex(character);
				if (index < 0 || index >= _sheet.TotatlTiles)
				{
					continue;
				}

				_sheet.SetRect(index);
				_sheet.Sprite.Position = new Vector2f(Position.X + offsetX, Position.Y + offsetY);
				_sheet.Sprite.Scale = new Vector2f(Scale, Scale);
				window.Draw(_sheet);

				offsetX += (_sheet.TileSize.X + Spacing.X) * Scale;
			}
		}

		private int GetIndex(char character)
		{
			return character - IndexOffset;
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			Draw(target);
		}
	}
}
