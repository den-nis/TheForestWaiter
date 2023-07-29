using System.Collections.Generic;

namespace TheForestWaiter.Game.Environment.Spawning.Pickers
{
	class RandomSpawnPicker : ISpawnPicker
	{
		private SpawnContext context;

		public RandomSpawnPicker()
		{
			context = IoC.GetInstance<SpawnContext>();
		}

		public IEnumerable<string> Generate(int budget, int wave)
		{
			var enemies = context.GetSpawnableEnemies(wave);
			int remainingBudget = budget;

			while (remainingBudget > 0)
			{	
				var enemy = Rng.Pick(enemies);
				var cost = context.GetCost(enemy);

				if (remainingBudget - cost >= 0)
				{
					remainingBudget -= cost;
					yield return enemy;
				}
			}
		}
	}
}