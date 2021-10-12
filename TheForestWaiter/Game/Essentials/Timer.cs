using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Game.Essentials
{
	public class Timer
	{
		private float _interval = 0;
		public bool Enabled { get; private set; } = true;

		public TimeSpan Interval => TimeSpan.FromSeconds(_interval);

		public float Time { get; private set; } 

		public event Action OnTick;

		public Timer(float interval) => SetInterval(interval);
		
		public Timer(TimeSpan interval) => SetInterval(interval);
		
		public Timer()
		{
			
		}

		public void Start() => Enabled = true;

		public void Stop() => Enabled = false;

		public void SetInterval(float interval)
		{
			_interval = interval;
		}

		public void SetInterval(TimeSpan interval)
		{
			_interval = (float)interval.TotalSeconds;
		}

		public void Reset() => Time = 0;
		
		public void Update(float time)
		{
			if (Enabled)
			{
				Time += time;

				while (_interval != 0 && Time > _interval)
				{
					Time -= _interval;
					OnTick?.Invoke();
				}
			}
		}
	}
}
