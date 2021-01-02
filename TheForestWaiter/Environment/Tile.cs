using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Environment
{
    public struct Tile
    {
        public bool Emtpy { get; set; }
        public bool Solid { get; set; }
        public int TileId { get; set; }

        public TileInfo ToTileInfo(Vector2f position)
        {
            return new TileInfo
            {
                Position = position,
                Tile = this,
            };
        }
    }

    public struct TileInfo
    {
        public Tile Tile { get; set; }
        public Vector2f Position { get; set; }
    }
}
