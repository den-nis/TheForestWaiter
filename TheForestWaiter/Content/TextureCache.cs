﻿using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    class TextureCache : ContentCache<Texture>
    {
        protected override ContentType Type => ContentType.Texture;

        public TextureCache(ContentConfig config) : base(config)
        {
        }

        protected override Texture LoadFromBytes(byte[] bytes)
        {
            return new Texture(bytes);
        }

        public Sprite CreateSprite(string name)
        {
            return new Sprite(Get(name));
        }

        public SpriteSheet CreateSpriteSheet(string name)
        {
            var meta = Config.Content[name];

            Debug.Assert(meta.TextureTileWidth != 0);
            Debug.Assert(meta.TextureTileHeight != 0);

            var sheet = new SpriteSheet(Get(name), meta.TextureTileWidth, meta.TextureTileHeight);

            if (meta.TextureHasTileSpacing)
                ApplySpacing(sheet);

            return sheet;
        }

        public AnimatedSprite CreateAnimatedSprite(string name)
        {
            var meta = Config.Content[name];
            var animation = new AnimatedSprite(Get(name), meta.TextureTileWidth, meta.TextureTileHeight, meta.TextureFramerate);
            if (meta.TextureHasTileSpacing)
                ApplySpacing(animation.Sheet);

            animation.AnimationEnd = animation.Sheet.TotatlTiles - 1;
            return animation;
        }

        private static void ApplySpacing(SpriteSheet sheet)
        {
            sheet.Spacing = new Vector2i(2, 2);
            sheet.Margin = new Vector2i(1, 1);
        }
    }
}
