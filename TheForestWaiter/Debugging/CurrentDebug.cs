using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging
{
	class CurrentDebug
	{
		public static IGameDebug Instance { get; private set; }

		public static void Set(IGameDebug instance)
		{
			if (Instance != null)
				throw new InvalidOperationException();

			Instance = instance;
		}
	}
}
