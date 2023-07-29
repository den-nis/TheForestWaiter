using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace TheForestWaiter.Game.Environment.Spawning
{
	[JsonConverter(typeof(StringEnumConverter))]
	internal enum SpawnSide
	{
		Left = -1,
		Right = 1,
	}
}
