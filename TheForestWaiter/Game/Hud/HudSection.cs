using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Game.Constants;

namespace TheForestWaiter.Game.Hud
{
	internal abstract class HudSection
	{
		public HudRegion Region { get; set; }
		public float Scale { get; private set; } = 1;
		public Vector2f Size { get; set; }
		public Vector2f Offset { get; set; }
		public bool Hidden { get; set; }

		public HudSection(float scale)
		{
			Scale = scale;
		}

		protected Vector2f ScaleVector => new(Scale, Scale);

		public abstract void Draw(RenderWindow window);

		public abstract void OnMouseMove(Vector2i mouse);

		public abstract bool IsMouseOnAnyButton();

		public abstract void OnPrimaryReleased();

		public abstract void OnPrimaryPressed();

		protected Vector2f GetPosition(RenderWindow window) => GetPosition(window.Size);

		protected Vector2f GetPosition(Camera window) => GetPosition(window.ViewportSize);

		private Vector2f GetPosition(Vector2u viewportSize)
		{
			var right = viewportSize.X - Size.X * Scale;
			var bottom = viewportSize.Y - Size.Y * Scale;

			return Region switch
			{
				HudRegion.TopLeft => new Vector2f(0, 0) + Offset,
				HudRegion.BottomLeft => new Vector2f(0, bottom) + Offset,
				HudRegion.BottomRight => new Vector2f(right, bottom) + Offset,
				HudRegion.TopRight => new Vector2f(right, 0) + Offset,
				_ => throw new KeyNotFoundException($"Cannot handle region {Region}"),
			};
		}

		public virtual void Update(float time)
		{
		}
	}
}
