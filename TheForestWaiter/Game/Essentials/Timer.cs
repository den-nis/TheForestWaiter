using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Game.Essentials
{
	public class Timer
	{
		private float _interval = 1;
		public bool Enabled { get; set; } = true;

		public TimeSpan Interval => TimeSpan.FromSeconds(_interval);
		private float _time;

		public event Action OnTick;

		public Timer(float interval) => SetInterval(interval);
		
		public Timer(TimeSpan interval) => SetInterval(interval);
		
		public Timer()
		{
			
		}

		public void SetInterval(float interval)
		{
			_interval = interval;
		}

		public void SetInterval(TimeSpan interval)
		{
			_interval = (float)interval.TotalSeconds;
		}

		public void Update(float time)
		{
			if (OnTick != null && Enabled)
			{
				_time += time;

				while (_time > _interval)
				{
					_time -= _interval;
					OnTick?.Invoke();
				}
			}
		}
	}
}
