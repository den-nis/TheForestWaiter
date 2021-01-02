using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Graphics
{
    public class SpriteSheet : Drawable
    {
        public Sprite Sprite { get; set; }

        public int CellWidth { get; private set; }
        public int CellHeight { get; private set; }

        public int CellsWidth  => (int)(Sprite.Texture.Size.X / CellWidth);
        public int CellsHeight => (int)(Sprite.Texture.Size.Y / CellHeight);
        public int TotatlCells => CellsWidth * CellsHeight;

        public bool MirrorX { get; set; } = false;
        public bool MirrorY { get; set; } = false;

        public SpriteSheet(Texture texture, int cellWidth, int cellHeight) 
        {
            Sprite = new Sprite(texture);
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public SpriteSheet(Sprite sheet, int cellWidth, int cellHeight)
        {
            Sprite = sheet;
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Sprite.Draw(target, states);
        }

        public void SetRect(int cellX, int cellY)
        {
            var rect = new IntRect(cellX * CellWidth, cellY * CellHeight, CellWidth, CellHeight);

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
