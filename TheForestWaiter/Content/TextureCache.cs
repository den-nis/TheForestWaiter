using SFML.Graphics;
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
            var meta = Config.GetByPath(name);

            Debug.Assert(meta.TextureConfig.TileWidth != 0);
            Debug.Assert(meta.TextureConfig.TileHeight != 0);

            var sheet = new SpriteSheet(Get(name), meta.TextureConfig.TileWidth, meta.TextureConfig.TileHeight);

            TryApplySpacing(sheet, meta);
            return sheet;
        }

        public AnimatedSprite CreateAnimatedSprite(string name)
        {
            var meta = Config.GetByPath(name);

            Debug.Assert(meta.TextureConfig.TileWidth != 0);
            Debug.Assert(meta.TextureConfig.TileHeight != 0);

            var animation = new AnimatedSprite(Get(name), meta.TextureConfig.TileWidth, meta.TextureConfig.TileHeight, meta.TextureConfig.Framerate);
            TryApplySpacing(animation.Sheet, meta);
            TryAddSections(animation, meta);

            animation.AnimationEnd = animation.Sheet.TotatlTiles - 1;
            return animation;
        }

        private static void TryAddSections(AnimatedSprite sprite, ContentMeta meta)
        {
            if (meta.TextureConfig.Sections != null)
            {
                sprite.Sections.AddRange(meta.TextureConfig.Sections);
            }
		}

        private static void TryApplySpacing(SpriteSheet sheet, ContentMeta meta)
        {
            if (meta.TextureConfig.HasTileSpacing)
            {
                sheet.Spacing = new Vector2i(2, 2);
                sheet.Margin = new Vector2i(1, 1);
            }
        }
    }
}
