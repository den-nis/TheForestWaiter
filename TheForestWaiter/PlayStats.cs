using TheForestWaiter.Game.Environment.Spawning;

namespace TheForestWaiter
{
	internal record PlayStats
	{
		public int Wave { get; set; }

		public void Harvest()
		{
			var scheduler = IoC.GetInstance<SpawnScheduler>();
			Wave = scheduler.WaveNumber;
		}
	}
}