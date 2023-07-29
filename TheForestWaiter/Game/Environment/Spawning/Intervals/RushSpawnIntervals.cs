using System;
using System.Collections.Generic;
using System.Linq;

namespace TheForestWaiter.Game.Environment.Spawning.Intervals
{
	internal class RushSpawnIntervals : ISpawnIntervals
	{
		public IEnumerable<float> Generate(int amount)
		{
			int majority = (int)Math.Floor(amount * 0.80f);
			int minority = amount - majority;

			IEnumerable<float> majorityIntervals = Enumerable.Range(0, majority).Select(_ => 0.10f / majority);
			IEnumerable<float> minorityIntervals = Enumerable.Range(0, minority).Select(_ => 0.90f / minority);

			var result = majorityIntervals.Concat(minorityIntervals);
			ISpawnIntervals.AssertGenerate(result, amount);
			return result;
		}
	}
}