using SFML.Graphics;
using SFML.System;

namespace TheForestWaiter.Graphics
{
	internal class SpriteSheetRect
	{
		public int TotatlTiles => GridSize.X * GridSize.Y;
		
		public bool MirrorX { get; set; } = false;
		public bool MirrorY { get; set; } = false;

		public Vector2i TextureSize { get; }
        public Vector2i CellSize { get; }

        /// <summary>
        /// The amount of cells for x and y
        /// </summary>
        public Vector2i GridSize { get; private set; }

		/// <summary>
		/// Spacing between cells
		/// </summary>
		public Vector2i Spacing { get; }

        /// <summary>
        /// Spacing around the whole grid
        public Vector2i Margin { get; set; }

        public SpriteSheetRect(Vector2i cellSize, Vector2i textureSize) : 
            this(cellSize, textureSize, new Vector2i(0,0), new Vector2i(0,0))
        {

        }

		public SpriteSheetRect(Vector2i cellSize, Vector2i textureSize, Vector2i spacing, Vector2i margin)
		{
            CellSize = cellSize;
			TextureSize = textureSize;
			Spacing = spacing;
            Margin = margin;

			ComputeGridSize();
		}

		public IntRect GetRect(int tileX, int tileY)
		{
			var left = (int)(Margin.X + (tileX * (CellSize.X + Spacing.X)));
			var top = (int)(Margin.Y + (tileY * (CellSize.Y + Spacing.Y)));

			var rect = new IntRect(
				left,
				top,
				CellSize.X,
				CellSize.Y
				);

			if (MirrorX)
			{
				rect.Left += CellSize.X;
				rect.Width = -rect.Width;
			}

			if (MirrorY)
			{
				rect.Top += CellSize.Y;
				rect.Height = -rect.Height;
			}

			return rect;
		}

		public IntRect GetRect(int index)
		{
			int x = index % GridSize.X;
			int y = index / GridSize.X;

			return GetRect(x, y);
		}

		private void ComputeGridSize()
		{
			var widthWithoutMargin = TextureSize.X - Margin.X * 2;
			var heightWithoutMargin = TextureSize.Y - Margin.Y * 2;

			var tileWidthWithSpacing = CellSize.X + Spacing.X;
			var tileHeightWithSpacing = CellSize.Y + Spacing.Y;

			GridSize = new Vector2i
			(
				 (int)((widthWithoutMargin + Spacing.X) / tileWidthWithSpacing),
				 (int)((heightWithoutMargin + Spacing.Y) / tileHeightWithSpacing)
			);
		}
	}
}