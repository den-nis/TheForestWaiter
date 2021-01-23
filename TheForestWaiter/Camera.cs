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
        private static Vector2f _baseSize = default;
        public static Vector2f BaseSize 
        {
            get => _baseSize;
            set
            {
                if (value != _baseSize)
                {
                    _baseSize = value;
                    SizeChanged();
                }
            }
        }

        private static float _scale = 1;
        public static float Scale 
        {
            get => _scale;
            set
            {
                if (value != _scale)
                {
                    _scale = value;
                    SizeChanged();
                }
            }
        } 

        public static Vector2f Size => BaseSize * Scale;

        public static Vector2f Position {
            get => Center - Size / 2;
            set => Center = value + Size / 2;
        }

        public static Vector2f Center { get; set; }

        private static void SizeChanged()
        {
            if (Size.X > GameSettings.Current.MaxWorldView.X || Size.Y > GameSettings.Current.MaxWorldView.Y)
            {
                var zoom = Math.Min(GameSettings.Current.MaxWorldView.Y / Size.Y, GameSettings.Current.MaxWorldView.X / Size.X);
                Scale *= zoom;
            }
        }

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
