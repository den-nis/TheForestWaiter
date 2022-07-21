using SFML.Graphics;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	class FontCache : ContentCache<Font>
	{
		protected override ContentType Type => ContentType.Font;

		public FontCache(ContentConfig config) : base(config)
		{
		}

		protected override Font LoadFromBytes(byte[] bytes) => new(bytes);
	}
}
