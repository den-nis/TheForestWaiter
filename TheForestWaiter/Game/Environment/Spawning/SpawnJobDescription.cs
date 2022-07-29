namespace TheForestWaiter.Game.Environment.Spawning
{
	internal class SpawnJobDescription
	{
		public float Time { get; set; }
		public string Creature { get; set; }
		public int Amount { get; set; }
		public float Delay { get; set; }
		public SpawnSide Side { get; set; }
	}
}
