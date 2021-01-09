using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Environment
{
    struct Tile
    {
        public bool Air { get; set; }
        public bool Solid { get; set; }

        public bool HasBackground => BackgroundTileId > 0;
        public bool HasMiddleground => MiddlegroundTileId > 0;
        public bool HasForeground => ForegroundTileId > 0;

        public short BackgroundTileId { get; set; } //For background blocks
        public short MiddlegroundTileId { get; set; } //For objects
        public short ForegroundTileId { get; set; }  

        public TileInfo ToTileInfo(Vector2f position)
        {
            return new TileInfo
            {
                Position = position,
                Tile = this,
            };
        }
    }

    struct TileInfo
    {
        public Tile Tile { get; set; }
        public Vector2f Position { get; set; }
    }
}
