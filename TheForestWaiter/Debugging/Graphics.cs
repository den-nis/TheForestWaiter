using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Environment;

namespace TheForestWaiter.Debugging
{
	static partial class GameDebug
	{
        private static readonly RectangleShape _box = new()
		{
            FillColor = Color.Transparent,
            OutlineThickness = 1,
        };

        private static readonly RectangleShape _redTileBox = new()
		{
            FillColor = new Color(255, 0, 0, 10),
            Size = new Vector2f(World.TILE_SIZE, World.TILE_SIZE),
        };

        private static readonly CircleShape _circle = new() 
        {
            FillColor = Color.Transparent,
            OutlineThickness = 1,
        };

        [Conditional("DEBUG")]
        public static void DrawWorldCollison(Vector2f pos)
        {
            if (GetVariable("draw_world_col", false))
            {
                DrawQueue.Enqueue((win) =>
                {
                    _redTileBox.Position = pos;
                    win.Draw(_redTileBox);
                });
            }
        }

        [Conditional("DEBUG")]
        public static void DrawHitBox(Vector2f position, Vector2f size, Color color)
        {
            if (GetVariable("draw_hitboxes", false))
                DrawQueue.Enqueue((win) =>
                {
                    _box.OutlineColor = color;
                    _box.OutlineThickness = Camera.Scale;
                    _box.Position = position;
                    _box.Size = size;
                    win.Draw(_box);
                });
        }

        [Conditional("DEBUG")]
        public static void DrawCircle(Vector2f position, float radius, Color color)
        {
            if (GetVariable("draw_hitboxes", false))
                DrawQueue.Enqueue((win) =>
                {
                    _circle.OutlineColor = color;
                    _circle.Position = position - new Vector2f(radius,radius);
                    _circle.Radius = radius;
                    _circle.OutlineThickness = Camera.Scale;
                    win.Draw(_circle);
                });
        }

        [Conditional("DEBUG")]
        public static void Draw(RenderWindow window)
        {
            while (DrawQueue.Count > 0)
            {
                DrawQueue.Dequeue()(window);
            }

            DrawHud(window);
        }

        [Conditional("DEBUG")]
        private static void DrawHud(RenderWindow window)
        {
            window.SetView(Camera.GetWindowView(window));

            if (GetVariable("draw_chunks", false))
                DrawChunks(window);

            StringBuilder sb = new();
            sb.AppendLine($"Fps: {Math.Round(_fps)}");
            sb.AppendLine($"Zoom: {Camera.Scale}");
            sb.AppendLine($"Bullets: {Game?.Objects?.Bullets?.Count()}");
            sb.AppendLine($"Health: {Game?.Objects?.Player?.Health}");
            sb.AppendLine($"Enemies: {Game?.Objects?.Enemies.Count()}");
            sb.AppendLine($"World particle index: {Game?.Objects?.WorldParticles.Index}");
            sb.AppendLine($"Speed X: {Game?.Objects?.Player?.RealSpeed.X}");
            sb.AppendLine($"Speed Y: {Game?.Objects?.Player?.RealSpeed.Y}");

            Text fpsText = new()
			{
                Position = new Vector2f(0, 0),
                DisplayedString = sb.ToString(),
                CharacterSize = 20,
                Font = GameContent.Fonts.Get("Fonts\\OpenSans-Regular.ttf"),
            };

            window.Draw(fpsText);
            window.SetView(Camera.GetView());
        }

        private static void DrawChunks(RenderWindow win)
        {
            var active = Game.Objects.Chunks.GetActiveChunks();

            RectangleShape chunkShape = new()
			{
                Size = new Vector2f(Chunks.CHUNK_WIDTH / Camera.Scale, Game.World.Tiles.GetLength(1) * World.TILE_SIZE / Camera.Scale),
                FillColor = Color.Transparent,
                OutlineThickness = -1,
            };

            for (int i = 0; i < Game.Objects.Chunks.TotalChunks; i++)
            {
                chunkShape.OutlineColor = Color.Green;
                if (active.Contains(i))
                    chunkShape.OutlineColor = Color.Red;

                chunkShape.Position = Camera.ToCamera(new Vector2f(i * Chunks.CHUNK_WIDTH, 0));
                win.Draw(chunkShape);
            }
        }
    }
}
