using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Static;

namespace TheForestWaiter
{
	internal record PlayStats
	{
		public int Wave { get; set; }

		public void Harvest()
		{
			var data = IoC.GetInstance<GameData>();
			var spawner = data.Objects.Environment.FirstOrDefault(x => x is Spawner);

			Wave = (spawner as Spawner)?.CurrentWave ?? 0;
		}
	}
}