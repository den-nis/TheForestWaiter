using System;
using System.Collections.Generic;
using System.Linq;

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

		public static T Pick<T>(IEnumerable<T> items)
		{
			return items.ElementAt(Rng.RangeInt(0, items.Count() - 1));
		}

		public static V PickPercentage<T, V>(IEnumerable<T> collection, Func<T, float> percentageFunc, Func<T, V> valueFunc)
		{
			var items = collection.Select(x => new {
				percentage = percentageFunc(x),
				value = valueFunc(x),
			});

			float value = Rng.Range(0, 1);
			float accumalted = 0;
			foreach (var item in items)
			{
				accumalted += item.percentage;
				if (accumalted >= value)
				{
					return item.value;
				}
			}

			return items.Last().value;
		}
	}
}
