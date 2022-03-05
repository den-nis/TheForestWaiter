using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Game.Constants;

namespace TheForestWaiter.Game.Hud
{
	internal abstract class HudSection
	{
		public HudRegion Region { get; set; }
		public float Scale { get; set; } = 1;
		public Vector2f Size { get; protected set; }
		public Vector2f Offset { get; set; }
		public bool Hidden { get; set; }

		protected Vector2f ScaleVector => new(Scale, Scale);

		public abstract void Draw(RenderWindow window);

		public abstract void Hover(Vector2f mouse);

		public abstract bool IsMouseCaptured();

		public abstract void OnPrimaryReleased();

		protected Vector2f GetPosition(RenderWindow window)
		{
			var right = window.Size.X - Size.X * Scale;
			var bottom = window.Size.Y - Size.Y * Scale;

			return Region switch
			{
				HudRegion.TopLeft => new Vector2f(0, 0) + Offset,
				HudRegion.BottomLeft => new Vector2f(0, bottom) + Offset,
				HudRegion.BottomRight => new Vector2f(right, bottom) + Offset,
				HudRegion.TopRight => new Vector2f(right, 0) + Offset,
				_ => throw new KeyNotFoundException($"Cannot handle region {Region}"),
			};
		}
	}
}
 