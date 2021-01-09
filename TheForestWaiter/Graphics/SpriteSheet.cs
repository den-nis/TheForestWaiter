using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Graphics
{
    class SpriteSheet : Drawable
    {
        public Sprite Sprite { get; set; }

        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        public int CellsWidth { get; private set; }
        public int CellsHeight { get; private set; }

        private Vector2f HalfMargin { get; set; }

        public int TotatlCells => CellsWidth * CellsHeight;

        private Vector2i _margin = default;
        private Vector2i _spacing = default;

        public void Refresh()
        {
            HalfMargin = new Vector2f(Margin.X/2f, Margin.Y/2f);

            var widthWithoutMargin = Sprite.Texture.Size.X - Margin.X * 2;
            var heightWithoutMargin = Sprite.Texture.Size.Y - Margin.Y * 2;

            CellsWidth  = (int)((widthWithoutMargin - Spacing.X / 2f) / (CellWidth + Spacing.X / 2f));
            CellsHeight = (int)((heightWithoutMargin - Spacing.Y / 2f) / (CellHeight + Spacing.Y / 2f));
        }

        public Vector2i Spacing {
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

        public SpriteSheet(Texture texture, int cellWidth, int cellHeight) 
        {
            Sprite = new Sprite(texture);
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            Refresh();
        }

        public SpriteSheet(Sprite sheet, int cellWidth, int cellHeight)
        {
            Sprite = sheet;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            Refresh();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Sprite.Draw(target, states);
        }

        public void SetRect(int cellX, int cellY)
        {
            var left = (int)(HalfMargin.X + (cellX * (CellWidth  + Spacing.X)));
            var top =  (int)(HalfMargin.Y + (cellY * (CellHeight + Spacing.Y)));

            var rect = new IntRect(
                left,
                top,
                CellWidth, 
                CellHeight
                );

            if (MirrorX)
            { 
                rect.Left += CellWidth;
                rect.Width = -rect.Width;
            }

            if (MirrorY)
            {
                rect.Top += CellHeight;
                rect.Height = -rect.Height;
            }

            Sprite.TextureRect = rect;
        }

        public void SetRect(int index)
        {
            int x = (index - 1) % CellsWidth;
            int y = (index - 1) / CellsWidth; 

            SetRect(x, y);
        }
    }
}
