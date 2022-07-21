using SFML.Graphics;
using SFML.System;

namespace TheForestWaiter
{
	internal static class WindowExtension
    {
		private static readonly RectangleShape _box = new()
		{
			FillColor = Color.Transparent,
		};

		public static void DrawHitBox(this RenderWindow window, Vector2f position, Vector2f size, Color color, float lineThickness)
        {
			_box.Position = position;
			_box.Size = size;
			_box.OutlineColor = color;
			_box.OutlineThickness = lineThickness;
			window.Draw(_box);
		}
    }
}
