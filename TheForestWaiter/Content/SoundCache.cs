using SFML.Audio;
using System.Collections.Generic;
using TheForestWaiter.Game.Sounds;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	class SoundCache : ContentCache<SoundBuffer>
	{
		protected override ContentType Type => ContentType.Sound;

		public SoundCache(ContentConfig config) : base(config)
		{
		}

		public Sound CreateSound(string name)
		{
			return new Sound(Get(name));
		}

		/// <summary>
		/// You can use {n} in the name to automatically load multiple sounds.
		/// Example: "Shoot{n}.wav"
		/// will load
		/// * Shoot1.wav
		/// * Shoot2.wav
		/// etc
		/// </summary>
		public GameSound CreateGameSound(string name)
		{
			List<SoundBuffer> sounds = new();
			if (name.Contains("{n}"))
			{
				int limit = 2;
				for (int i = 0; i < limit; i++)
				{
					var replaced = name.Replace("{n}", i.ToString());
					if (Config.Content.Exists(x => x.Path == replaced))
					{
						sounds.Add(Get(replaced));
						limit++;
					}
				}
			}
			else
			{
				sounds.Add(Get(name));
			}

			return new GameSound(sounds.ToArray());
		}

		protected override SoundBuffer LoadFromBytes(byte[] bytes) => new(bytes);
	}
}
