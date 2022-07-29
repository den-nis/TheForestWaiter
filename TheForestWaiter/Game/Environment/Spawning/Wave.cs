using System.Collections.Generic;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal class Wave
	{
		public IEnumerable<SpawnJobDescription> Jobs { get; set; }
	}
}
