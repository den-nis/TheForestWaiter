using SFML.Audio;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	class SoundCache : ContentCache<SoundBuffer>
	{
		protected override ContentType Type => ContentType.Sound;

		public SoundCache(ContentConfig config) : base(config)
		{
		}

		protected override SoundBuffer LoadFromBytes(byte[] bytes) => new(bytes);
	}
}
