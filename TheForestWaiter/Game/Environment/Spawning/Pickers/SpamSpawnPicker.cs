using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Game.Environment.Spawning.Pickers
{
	class SpamSpawnPicker : ISpawnPicker
	{
		private SpawnContext context;
		private IDebug debug;

		public SpamSpawnPicker()
		{
			context = IoC.GetInstance<SpawnContext>();
			debug = IoC.GetInstance<IDebug>();
		}

		public IEnumerable<string> Generate(int budget, int wave)
		{
			var enemies = context.GetSpawnableEnemies(wave);
			var enemy = Rng.Pick(enemies);
			int cost = context.GetCost(enemy);
			int amount = budget / cost;

			return Enumerable.Range(0, amount).Select(_ => enemy);
		}
	}
}