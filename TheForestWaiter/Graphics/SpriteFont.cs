using SFML.Graphics;
using SFML.System;
using System;

namespace TheForestWaiter.Graphics
{
	internal class SpriteFont : Drawable
	{
		public SpriteSheet Sheet { get; set; }
		public Vector2f Position { get; set; }
		public Vector2f Spacing { get; set; } = new Vector2f(1, 1);
		public Color Color { get; set; } = Color.White;

		public float Scale { get; set; } = 1;
		public int IndexOffset { get; set; } = '0';
		public Func<char, int> CustomIndexMapping { private get; set; }

		private string _text = string.Empty;

		public SpriteFont(SpriteSheet sheet)
		{
			Sheet = sheet;
		}

		public void SetText(string text)
		{
			_text = text;
		}

		public void Draw(RenderTarget window)
		{
			float offsetX = 0;
			float offsetY = 0;

			Sheet.Sprite.Color = Color;
			for (int i = 0; i < _text.Length; i++)
			{
				char character = _text[i];

				if (character == '\n')
				{
					offsetY += (Sheet.TileSize.Y + Spacing.Y) * Scale;
					offsetX = 0;
					continue;
				}

				int index = GetIndex(character);
				if (index < 0 || index >= Sheet.TotatlTiles)
				{
					continue;
				}

				Sheet.SetRect(index);
				Sheet.Sprite.Position = new Vector2f(Position.X + offsetX, Position.Y + offsetY);
				Sheet.Sprite.Scale = new Vector2f(Scale, Scale);
				window.Draw(Sheet);

				offsetX += (Sheet.TileSize.X + Spacing.X) * Scale;
			}
		}

		private int GetIndex(char character)
		{
			if (CustomIndexMapping != null)
			{
				return CustomIndexMapping(character);
			}
			else
			{
				return character - IndexOffset;
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			Draw(target);
		}
	}
}
