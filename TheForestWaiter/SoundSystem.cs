using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game;

namespace TheForestWaiter
{
	internal class SoundSystem : IDisposable
	{
		// Long sounds get enqueued to the secondary queue. 
		// This is so that long sounds can't stop short sounds from being dequeued 

		private const float SECONDARY_THRESHOLD = 2f;
		private const string SET_WILDCARD = "{n}";

		private readonly ContentSource _content;

		private readonly Dictionary<string, string[]> _setCache = new();
		private readonly Queue<Sound> _primarySoundQueue = new();
		private readonly Queue<Sound> _secondarySoundQueue = new();
		private readonly IEnumerable<Queue<Sound>> _queues;

		public SoundSystem(ContentSource content)
		{
			_content = content;
			_queues = new[] { _primarySoundQueue, _secondarySoundQueue };
		}

		public void Play(SoundInfo soundInfo)
		{
			Debug.Assert(soundInfo != null, "Avoid using null soundinfo");

			if (soundInfo != null && !soundInfo.IsSilent())
			{
				var files = GetFileNamesCached(soundInfo.Identifier);
				PlayInternal(soundInfo, files);
			}
		}

		private void PlayInternal(SoundInfo info, IEnumerable<string> files)
		{
			var array = files.ToArray();
			var file = array[Rng.RangeInt(0, array.Length - 1)];

			var buffer = _content.Sounds.Get(file);

			var sound = new Sound(buffer)
			{
				Pitch = info.Pitch + Rng.Var(info.PitchVariation),
				Volume = info.Volume + Rng.Var(info.VolumeVariation)
			};

			sound.Play();

			if ((buffer.Samples.Length / (float)buffer.SampleRate) < SECONDARY_THRESHOLD)
			{
				_primarySoundQueue.Enqueue(sound);
			}
			else
			{
				_secondarySoundQueue.Enqueue(sound);
			}

			CleanupQueue();
		}

		private IEnumerable<string> GetFileNamesCached(string identifier)
		{
			if (_setCache.TryGetValue(identifier, out var names))
			{
				return names;
			}
			else
			{
				names = GetFileNames(identifier).ToArray();
				_setCache[identifier] = names;
				return names;
			}

		}

		private IEnumerable<string> GetFileNames(string identifier)
		{
			if (identifier.Contains(SET_WILDCARD))
			{
				int total = 0;
				int max = 2;
				for (int i = 0; i < max; i++)
				{
					var path = identifier.Replace(SET_WILDCARD, i.ToString());
					if (_content.Config.HasFile(path))
					{
						total++;
						max++;
						yield return path;
					}
				}

				Debug.Assert(total != 0, $"Could not find files with identifier : \"{identifier}\" ");
			}
			else
			{
				yield return identifier;
			}
		}

		public void CleanupQueue()
		{
			foreach (var queue in _queues)
			{
				while (queue.Any() && queue.Peek().Status == SoundStatus.Stopped)
				{
					var sound = queue.Dequeue();
					sound.Dispose();
				}
			}
		}

		public void Dispose()
		{
			foreach (var sound in _queues.SelectMany(x => x))
			{
				sound.Dispose();
			}
		}
	}
}
