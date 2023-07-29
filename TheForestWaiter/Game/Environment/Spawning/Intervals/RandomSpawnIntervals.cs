using System.Collections.Generic;
using System.Linq;

namespace TheForestWaiter.Game.Environment.Spawning.Intervals
{
	internal class RandomSpawnIntervals : ISpawnIntervals
	{
		public IEnumerable<float> Generate(int amount)
		{
			float sum = 0;
			IEnumerable<float> numbers = null;

			numbers = Enumerable.Range(0, amount).Select(_ => Rng.Float()).ToArray();
			sum = numbers.Sum();

			var result = numbers.Select(x => x / sum);
			ISpawnIntervals.AssertGenerate(result, amount);
			return result;
		}
	}
}