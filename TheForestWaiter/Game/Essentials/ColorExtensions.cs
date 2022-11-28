using SFML.Graphics;

namespace TheForestWaiter.Game.Essentials
{
	public static class ColorExtensions
	{
		public static string ToColorCode(this Color vec)
		{
			return $"[{vec.R},{vec.G},{vec.B}]";
		}
	}
}