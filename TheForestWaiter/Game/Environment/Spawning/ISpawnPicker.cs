using System.Collections.Generic;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal interface ISpawnPicker
	{
		public IEnumerable<string> Generate(int budget, int wave);
	}
}