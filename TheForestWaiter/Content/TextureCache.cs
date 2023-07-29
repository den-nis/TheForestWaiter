using SFML.Graphics;
using SFML.System;
using System.Diagnostics;
using TheForestWaiter.Graphics;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	class TextureCache : ContentCache<Texture>
	{
		protected override ContentType Type => ContentType.Texture;

		private readonly Vector2i TILES_SPACING = new Vector2i(2,2);
		private readonly Vector2i TILES_MARGIN = new Vector2i(1,1);

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

			var size = new Vector2i(meta.TextureConfig.TileWidth, meta.TextureConfig.TileHeight);
			var sheet = meta.TextureConfig.HasTileSpacing 
				? new SpriteSheet(Get(name), size, TILES_SPACING, TILES_MARGIN)
				: new SpriteSheet(Get(name), size);

			return sheet;
		}

		public AnimatedSprite CreateAnimatedSprite(string name)
		{
			var meta = Config.GetByPath(name);

			Debug.Assert(meta.TextureConfig.TileWidth != 0);
			Debug.Assert(meta.TextureConfig.TileHeight != 0);

			var size = new Vector2i(meta.TextureConfig.TileWidth, meta.TextureConfig.TileHeight);
			var animation = meta.TextureConfig.HasTileSpacing 
				? new AnimatedSprite(Get(name), size, TILES_SPACING, TILES_MARGIN, meta.TextureConfig.Framerate)
				: new AnimatedSprite(Get(name), size, meta.TextureConfig.Framerate);

			TryAddSections(animation, meta);
			animation.AnimationEnd = animation.Sheet.Rect.TotatlTiles - 1;
			return animation;
		}

		private static void TryAddSections(AnimatedSprite sprite, ContentMeta meta)
		{
			if (meta.TextureConfig.Sections != null)
			{
				sprite.Sections.AddRange(meta.TextureConfig.Sections);
			}
		}
	}
}
