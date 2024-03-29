﻿using System;
using System.Diagnostics;
using System.IO;

namespace TheForestWaiter.Content
{
	class Program
	{
		static void Main(string[] args)
		{
			string source = Path.GetFullPath(args[0]);
			string output = Path.GetFullPath(args[1]);
			Console.WriteLine($"Content source: {source}");
			Console.WriteLine($"Content output: {output}");

			Stopwatch timer = Stopwatch.StartNew();
			ContentBuilder builder = new(source);
			builder.Build(output);
			Console.WriteLine($"Exported to {output}");
			Console.WriteLine($"Content build finished in {timer.Elapsed.Minutes} minutes and {timer.Elapsed.TotalSeconds} seconds");
		}
	}
}
