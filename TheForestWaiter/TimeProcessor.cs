using System;

namespace TheForestWaiter
{
	internal class TimeProcessor
	{
		public float LockDelta { get; set; } = -1;
		public bool LagLimit { get; set; } = true;
		public float TimeScale { get; set; } = 1;
		public float TimeDifference { get; set; }
		public float Framerate => 1 / AverageDelta;

		public float AverageDelta = 16f * Time.MILLISECONDS;
		public float AverageTimeDifference = 0;

		public float Process(float delta)
		{
			delta *= TimeScale;
			var expected = delta;

			if (LockDelta > 0)
				delta = LockDelta;

			if (LagLimit)
				delta = Math.Min(delta, AverageDelta * 1.1f);

			TimeDifference = (delta / expected) - 1;
			AverageTimeDifference = (AverageTimeDifference * 2 + TimeDifference) / 3;
			AverageDelta = (AverageDelta * 2 + delta) / 3;

			return delta;
		}
	}
}
