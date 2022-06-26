using Newtonsoft.Json.Linq;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal class WaveSettings
	{
		public Vector2f LeftSpawn { get; set; }
		public Vector2f RightSpawn { get; set; }

		public Wave[] Waves { get; set; }

		public static WaveSettings FromJson(string json)
		{
			WaveSettings instance = new();
			JToken obj = JObject.Parse(json);

			static string Lower(string input) => $"{char.ToLower(input[0])}{input[1..]}";

			instance.LeftSpawn = ParseVector(obj[Lower(nameof(LeftSpawn))]);
			instance.RightSpawn = ParseVector(obj[Lower(nameof(RightSpawn))]);

			instance.Waves = obj[Lower(nameof(Waves))].ToObject<Wave[]>();

			return instance;
		}

		private static Vector2f ParseVector(JToken property)
		{
			var x = (float)property.First;
			var y = (float)property.Last;
			return new Vector2f(x, y);
		}
	}
}
