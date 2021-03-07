
using System;
using System.IO;

namespace TheForestWaiter.Debugging
{
	public static class Crash
	{
		public static void Now(string message)
		{
			File.WriteAllText($"Crashlog{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}.txt", message);
		}

		public static void Now(Exception e)
		{
			Now(e.ToString());
			System.Environment.Exit(1);
		}
	}
}