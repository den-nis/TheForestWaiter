﻿using Newtonsoft.Json;
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
            Sheet = new SpriteSheet(GameContent.GetTexture("Content.Textures.World.world.png"), TILE_SIZE, TILE_SIZE)
            {
                Spacing = new Vector2i(2, 2),
                Margin = new Vector2i(1, 1)
            };
        }

        public static World LoadFromMap(Map map)
        {
            var world = new World
            {
                Tiles = new Tile[map.Layers.First().Width, map.Layers.First().Height],
            };


            var layerLookup = map.Layers.ToDictionary(l => l.Name);

            world.LoadTileLayers
            (
                layerLookup["Background"],
                layerLookup["Middleground"],
                layerLookup["Foreground"],
                layerLookup["Solid"]
            );

            return world;
        }

        public IEnumerable<TileInfo> GetTilesInArea(Vector2f size, Vector2f center)
        {
            var topLeft = center - size / 2;

            return GetSolidTilesInRect(new FloatRect(
                topLeft,
                size));
        }

        public IEnumerable<TileInfo> GetSolidTilesInFrame(FloatRect rect)
        {
            return GetSolidTilesInRect(rect);
        }

        public IEnumerable<TileInfo> GetSolidTilesInRect(FloatRect rect)
        {
            var r = GetTileBounds(rect);

            for (int y = r.Top; y < r.Top + r.Height; y++)
            {
                for (int x = r.Left; x < r.Left + r.Width; x++)
                {
                    if (Tiles[x, y].Solid)
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

        public bool TouchingSolid(Vector2f location)
        {
            var tx = (int)Math.Floor(location.X / TILE_SIZE);
            var ty = (int)Math.Floor(location.Y / TILE_SIZE);

            if (tx >= Tiles.GetLength(0) || tx < 0 || ty >= Tiles.GetLength(1) || ty < 0)
                return false;

            return Tiles[tx, ty].Solid;
        }

        public void Draw(RenderWindow win, FloatRect rect, bool onlyForeground)
        {
            var r = GetTileBounds(rect);

            int i = 0;

            for (int y = r.Top; y < r.Top + r.Height; y++)
            {
                for (int x = r.Left; x < r.Left + r.Width; x++)
                {
                    if (!Tiles[x, y].Air)
                    {
                        Sheet.Sprite.Position = new Vector2f(x * TILE_SIZE, y * TILE_SIZE);

                        if (onlyForeground)
                        {
                            if (Tiles[x, y].HasForeground)
                            {
                                Sheet.SetRect(Tiles[x, y].ForegroundTileId);
                                win.Draw(Sheet.Sprite);
                            }
                        }
                        else
                        {
                            if (Tiles[x, y].HasBackground)
                            {
                                Sheet.SetRect(Tiles[x, y].BackgroundTileId);
                                win.Draw(Sheet.Sprite);
                            }

                            if (Tiles[x, y].HasMiddleground)
                            {
                                Sheet.SetRect(Tiles[x, y].MiddlegroundTileId);
                                win.Draw(Sheet.Sprite);
                            }
                        }
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

        private void LoadTileLayers(Layer background, Layer middleground, Layer foreground, Layer solid)
        {
            for (int i = 0; i < background.Data.Length; i++)
            {
                ref Tile t = ref Tiles[i % background.Width, i / background.Width];
                ref int backId   = ref background.Data[i];
                ref int middleId = ref middleground.Data[i];
                ref int foreId   = ref foreground.Data[i];
                ref int solidId  = ref solid.Data[i];

                t.Air =
                    backId   == 0 &&
                    middleId == 0 &&
                    foreId   == 0;

                t.Solid = solidId > 0;

                t.BackgroundTileId   = (short)backId;
                t.MiddlegroundTileId = (short)middleId;
                t.ForegroundTileId   = (short)foreId;
            }
        }
    }
}
