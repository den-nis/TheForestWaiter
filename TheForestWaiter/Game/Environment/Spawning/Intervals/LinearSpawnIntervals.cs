using System.Collections.Generic;
using System.Linq;

namespace TheForestWaiter.Game.Environment.Spawning.Intervals
{
	internal class LinearSpawnIntervals : ISpawnIntervals
	{
		public IEnumerable<float> Generate(int amount)
		{
			var result = Enumerable.Range(0, amount).Select(_ => 1f / (float)amount);
			ISpawnIntervals.AssertGenerate(result, amount);
			return result;
		}
	}
}