using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Graphics
{
	internal class SpriteSheet : Drawable
	{
		public Sprite Sprite { get; set; }
		public SpriteSheetRect Rect { get; }

		public SpriteSheet(Texture texture, Vector2i cellSize) : this(new Sprite(texture), cellSize)
		{

		}

		public SpriteSheet(Texture texture, Vector2i cellSize, Vector2i spacing, Vector2i margin)
			: this(new Sprite(texture), cellSize, spacing, margin)
		{

		}

		public SpriteSheet(Sprite sheet, Vector2i cellSize)
		{
			Sprite = sheet;
			Rect = new SpriteSheetRect(cellSize, sheet.Texture.Size.ToVector2i());
		}

		public SpriteSheet(Sprite sheet, Vector2i cellSize, Vector2i spacing, Vector2i margin)
		{
			Sprite = sheet;
			Rect = new SpriteSheetRect(cellSize, sheet.Texture.Size.ToVector2i(), spacing, margin);
		}

		public void SetRect(int tileX, int tileY) => Sprite.TextureRect = Rect.GetRect(tileX, tileY);

		public void SetRect(int index) => Sprite.TextureRect = Rect.GetRect(index);

		public void Draw(RenderTarget target, RenderStates states)
		{
			Sprite.Draw(target, states);
		}
	}
}
