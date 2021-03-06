using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Environment;

namespace TheForestWaiter.Debugging
{
	static partial class GameDebug
	{
        static RectangleShape Box { get; } = new RectangleShape
        {
            FillColor = Color.Transparent,
            OutlineThickness = 1,
        };

        static RectangleShape RedTileBox { get; } = new RectangleShape
        {
            FillColor = new Color(255, 0, 0, 10),
            Size = new Vector2f(World.TILE_SIZE, World.TILE_SIZE),
        };

        static CircleShape Circle { get; } = new CircleShape 
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
                    RedTileBox.Position = pos;
                    win.Draw(RedTileBox);
                });
            }
        }

        [Conditional("DEBUG")]
        public static void DrawHitBox(Vector2f position, Vector2f size, Color color)
        {
            if (GetVariable("draw_hitboxes", false))
                DrawQueue.Enqueue((win) =>
                {
                    Box.OutlineColor = color;
                    Box.Position = position;
                    Box.Size = size;
                    win.Draw(Box);
                });
        }

        [Conditional("DEBUG")]
        public static void DrawCircle(Vector2f position, float radius, Color color)
        {
            if (GetVariable("draw_hitboxes", false))
                DrawQueue.Enqueue((win) =>
                {
                    Circle.OutlineColor = color;
                    Circle.Position = position - new Vector2f(radius,radius);
                    Circle.Radius = radius;
                    win.Draw(Circle);
                });
        }

        [Conditional("DEBUG")]
        public static void Draw(RenderWindow window)
        {
            while (DrawQueue.Count > 0)
            {
                DrawQueue.Dequeue()(window);
            }
        }


        [Conditional("DEBUG")]
        public static void DrawUI(RenderWindow window)
        {
            if (GetVariable("draw_chunks", false))
                DrawChunksUI(window);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Fps: {Math.Round(Fps)}");
            sb.AppendLine($"Zoom: {Camera.Scale}");
            sb.AppendLine($"Bullets: {Game?.Objects?.Bullets?.Count()}");
            sb.AppendLine($"Health: {Game?.Objects?.Player?.Health}");
            sb.AppendLine($"Enemies: {Game?.Objects?.Enemies.Count()}");
            sb.AppendLine($"World particle index: {Game?.Objects?.WorldParticles.Index}");
            sb.AppendLine($"Speed X: {Game?.Objects?.Player?.RealSpeed.X}");
            sb.AppendLine($"Speed Y: {Game?.Objects?.Player?.RealSpeed.Y}");

            Text fpsText = new Text
            {
                Position = new Vector2f(0, 0),
                DisplayedString = sb.ToString(),
                CharacterSize = 20,
                Font = GameContent.Fonts.Get("Fonts\\OpenSans-Regular.ttf"),
            };

            window.Draw(fpsText);
        }

        private static void DrawChunksUI(RenderWindow win)
        {
            var active = Game.Objects.Chunks.GetActiveChunks();

            RectangleShape chunkShape = new RectangleShape
            {
                Size = new Vector2f(Chunks.CHUNK_WIDTH / Camera.Scale, Game.World.Tiles.GetLength(1) * World.TILE_SIZE / Camera.Scale),
                FillColor = Color.Transparent,
                OutlineThickness = -1,
            };

            for (int i = 0; i < Game.Objects.Chunks.TotalChunks; i++)
            {
                chunkShape.OutlineColor = Color.Green;
                if (active.Contains(i))
                {
                    chunkShape.OutlineColor = Color.Red;
                }

                chunkShape.Position = Camera.ToCamera(new Vector2f(i * Chunks.CHUNK_WIDTH, 0));
                win.Draw(chunkShape);
            }
        }
    }
}
