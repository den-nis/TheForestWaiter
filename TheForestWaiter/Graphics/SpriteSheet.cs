using SFML.Graphics;
using SFML.System;

namespace TheForestWaiter.Graphics
{
	internal class SpriteSheet : Drawable
	{
		public Sprite Sprite { get; set; }

		public Vector2i Tiles { get; private set; }
		public Vector2i TileSize { get; }

		public int TotatlTiles => Tiles.X * Tiles.Y;

		private Vector2i _margin = default;
		private Vector2i _spacing = default;

		public Vector2i Spacing
		{
			get => _spacing;
			set
			{
				_spacing = value;
				Refresh();
			}
		}
		public Vector2i Margin
		{
			get => _margin;
			set
			{
				_margin = value;
				Refresh();
			}
		}

		public bool MirrorX { get; set; } = false;
		public bool MirrorY { get; set; } = false;

		//TODO: Why not use Vector2i
		public SpriteSheet(Texture texture, int tileWidth, int tileHeight) : this(new Sprite(texture), tileWidth, tileHeight)
		{

		}

		public SpriteSheet(Sprite sheet, int tileWidth, int tileHeight)
		{
			Sprite = sheet;
			TileSize = new Vector2i(tileWidth, tileHeight);
			Refresh();
			SetRect(1);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			Sprite.Draw(target, states);
		}

		public void SetRect(int tileX, int tileY)
		{
			var left = (int)(Margin.X + (tileX * (TileSize.X + Spacing.X)));
			var top = (int)(Margin.Y + (tileY * (TileSize.Y + Spacing.Y)));

			var rect = new IntRect(
				left,
				top,
				TileSize.X,
				TileSize.Y
				);

			if (MirrorX)
			{
				rect.Left += TileSize.X;
				rect.Width = -rect.Width;
			}

			if (MirrorY)
			{
				rect.Top += TileSize.Y;
				rect.Height = -rect.Height;
			}

			Sprite.TextureRect = rect;
		}

		public void SetRect(int index)
		{
			int x = index % Tiles.X;
			int y = index / Tiles.X;

			SetRect(x, y);
		}

		private void Refresh()
		{
			var widthWithoutMargin = Sprite.Texture.Size.X - Margin.X * 2;
			var heightWithoutMargin = Sprite.Texture.Size.Y - Margin.Y * 2;

			var tileWidthWithSpacing = TileSize.X + Spacing.X;
			var tileHeightWithSpacing = TileSize.Y + Spacing.Y;

			Tiles = new Vector2i
			(
				 (int)((widthWithoutMargin + Spacing.X) / tileWidthWithSpacing),
				 (int)((heightWithoutMargin + Spacing.Y) / tileHeightWithSpacing)
			);
		}
	}
}
