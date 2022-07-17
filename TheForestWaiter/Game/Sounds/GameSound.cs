using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Audio;

namespace TheForestWaiter.Game.Sounds
{
	internal class GameSound
	{
		public float PitchVariation { get; set; } = 0;
		public float VolumeVariation { get; set; } = 0;
		public float Volume { get; set; } = 50;

		private readonly List<SoundBuffer> _buffers;
		private readonly Queue<Sound> _soundQueue = new();
		
		public GameSound(SoundBuffer buffer)
		{
			_buffers = new List<SoundBuffer> { buffer };
		}

		public GameSound(params SoundBuffer[] sounds)
		{
			_buffers = sounds.ToList();
		}

		public void Play()
		{
			var sound = new Sound(_buffers[Rng.RangeInt(0, _buffers.Count - 1)]);
			sound.Pitch += Rng.Var(PitchVariation);
			sound.Volume = Volume + Rng.Var(VolumeVariation);
			sound.Play();

			_soundQueue.Enqueue(sound);
			CleanupQueue();
		}

		public void CleanupQueue()
		{
			while(_soundQueue.Peek().Status == SoundStatus.Stopped)
			{
				var sound = _soundQueue.Dequeue();
				sound.Dispose();
			}
		}
	}
}
