using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Environment
{
    class World
    {
        public const int TILE_SIZE = 32;
        public Tile[,] Tiles { get; set; }
        private SpriteSheet Sheet { get; set; }

        private World()
        {
            Sheet = new SpriteSheet(GameContent.GetTexture("Content.Textures.World.world.png"), TILE_SIZE, TILE_SIZE);
        }

        public static World LoadFromMap(Map map)
        {
            var world = new World
            {
                Tiles = new Tile[map.Layers.First().Width, map.Layers.First().Height],
            };

            var layerLookup = map.Layers.ToDictionary(k => k.Type);
            world.LoadTileLayer(map.Layers.First(k => k.Type == "tilelayer"));

            return world;
        }

        public IEnumerable<TileInfo> GetTilesInArea(Vector2f size, Vector2f center)
        {
            var topLeft = center - size / 2;

            return GetTilesInRect(new FloatRect(
                topLeft,
                size));
        }

        public IEnumerable<TileInfo> GetNonEmptyTilesInFrame(FloatRect rect)
        {
            return GetTilesInRect(rect);
        }

        public IEnumerable<TileInfo> GetTilesInRect(FloatRect rect)
        {
            var r = GetTileBounds(rect);

            for (int y = r.Top; y < r.Top + r.Height; y++)
            {
                for (int x = r.Left; x < r.Left + r.Width; x++)
                {
                    if (!Tiles[x, y].Emtpy)
                    {
                        yield return Tiles[x, y].ToTileInfo(new Vector2f(x * TILE_SIZE, y * TILE_SIZE));
                    }
                }
            }
        }

        public TileInfo? GetTileInfoAt(Vector2f location)
        {
            var x = (int)Math.Floor(location.X / TILE_SIZE);
            var y = (int)Math.Floor(location.Y / TILE_SIZE);

            if (x < 0 || y < 0 || x > Tiles.GetLength(0) - 1 || y > Tiles.GetLength(1) - 1)
                return null;

            Tile tile = Tiles[x, y];

            return tile.ToTileInfo(new Vector2f(x * TILE_SIZE, y * TILE_SIZE));
        }

        public bool PointHit(Vector2f location)
        {
            var tx = (int)Math.Floor(location.X / TILE_SIZE);
            var ty = (int)Math.Floor(location.Y / TILE_SIZE);

            if (tx > Tiles.GetLength(0) || tx < 0 || ty > Tiles.GetLength(1) || ty < 0)
                return false;

            return !Tiles[tx, ty].Emtpy;
        }

        public void Draw(RenderWindow win, FloatRect rect)
        {
            var r = GetTileBounds(rect);

            for (int y = r.Top; y < r.Top + r.Height; y++)
            {
                for (int x = r.Left; x < r.Left + r.Width; x++)
                {
                    if (!Tiles[x, y].Emtpy)
                    {
                        Sheet.SetRect(Tiles[x, y].TileId);
                        Sheet.Sprite.Position = new Vector2f(x * TILE_SIZE, y * TILE_SIZE);
                        win.Draw(Sheet.Sprite);
                    }
                }
            }
        }

        private IntRect GetTileBounds(FloatRect bounds)
        {
            int left = (int)Math.Floor(bounds.Left / TILE_SIZE);
            int top = (int)Math.Floor(bounds.Top / TILE_SIZE);

            int right = (int)Math.Ceiling((bounds.Left + bounds.Width) / TILE_SIZE);
            int bottom = (int)Math.Ceiling((bounds.Top + bounds.Height) / TILE_SIZE);

            left = Math.Max(0, left);
            top = Math.Max(0, top);

            right = Math.Min(right, Tiles.GetLength(0));
            bottom = Math.Min(bottom, Tiles.GetLength(1));

            return new IntRect(new Vector2i(left, top), new Vector2i(right - left, bottom - top));
        }

        private void LoadTileLayer(Layer layer)
        {
            for (int i = 0; i < layer.Data.Length; i++)
            {
                ref Tile t = ref Tiles[i % layer.Width, i / layer.Width];

                if (layer.Data[i] == 0)
                {
                    t.Emtpy = true;
                }
                else
                {
                    t.TileId = layer.Data[i];
                }
            }
        }
    }
}
