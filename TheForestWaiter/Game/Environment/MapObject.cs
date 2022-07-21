using SFML.System;

namespace TheForestWaiter.Game.Environment
{
	sealed internal class MapObject
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public float Rotation { get; set; }
		public string Class { get; set; }
		public bool Visible { get; set; }
		public float X { get; set; }
		public float Y { get; set; }

		public MapProperties[] Properties { get; set; }

		public Vector2f Position => new(X, Y);
	}
}
