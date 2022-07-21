using System;

namespace TheForestWaiter.Game
{
	internal static class Rng
	{
		private static Random Random { get; set; } = new Random();

		public static bool Bool()
		{
			return Float() > .5f;
		}

		public static float Float()
		{
			return (float)Random.NextDouble();
		}

		public static float Range(float min, float max)
		{
			return min + Float() * (max - min);
		}

		public static int RangeInt(int min, int max)
		{
			return (int)Math.Round(min + Float() * ((float)max - min));
		}

		public static float Angle() => Range(0, (float)(Math.PI * 2));

		public static float Var(float variation)
		{
			return Range(-variation, variation);
		}

		public static float Var(float source, float variation)
		{
			return source + Range(-variation, variation);
		}
	}
}
