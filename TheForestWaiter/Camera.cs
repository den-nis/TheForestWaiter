using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
    public static class Camera
    {
        public static Vector2f BaseSize { get; set; }

        public static float Scale { get; set; } = 1;

        public static Vector2f Size => BaseSize * Scale;

        public static Vector2f Position {
            get => Center - Size / 2;
            set => Center = value + Size / 2;
        }

        public static Vector2f Center { get; set; }

        public static Vector2f ToWorld(Vector2f view)
        {
            return Position + (view * Scale);
        }

        public static Vector2f ToCamera(Vector2f world)
        {
            return (world - Position) / Scale;
        }

        public static View GetView()
        {
            return new View(Center, Size);
        }
    }
}
