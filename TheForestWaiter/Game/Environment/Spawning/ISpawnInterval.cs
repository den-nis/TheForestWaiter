using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal interface ISpawnIntervals
	{
		/// <summary>
		/// Returns the intervals used for spawning. The total should always add up to about 1
		/// </summary>
		public IEnumerable<float> Generate(int amount);

		protected static void AssertGenerate(IEnumerable<float> result, int amount) 
		{
			Debug.Assert(result.Count() == amount, "Incorrect result") ;
			Debug.Assert(Math.Abs(result.Sum() - 1) < 0.01f, "Inaccurate result, error : " + Math.Abs(result.Sum() - 1));
		}
	}
}