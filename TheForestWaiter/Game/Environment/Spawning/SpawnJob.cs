namespace TheForestWaiter.Game.Environment.Spawning
{
	internal class SpawnJob
	{
		/// <summary>
		/// Name of the creatue to spawn
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Spawn rate
		/// </summary>
		public float Rate { get; set; }

		/// <summary>
		/// Per second increase in spawn rate
		/// </summary>
		public float Increase { get; set; }
	}
}
