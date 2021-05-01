using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Debugging;

namespace TheForestWaiter
{
    public static class Camera
    {
        private static readonly GameSettings _settings = GameSettings.Current;

        private const float MOVE_STRENGTH = 50;
        private const float ZOOM_STRENGTH = 10;

        public static Vector2f TargetPosition { get; set; }
        public static float TargetScale { get; set; } = 1;

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
            private set
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
            if (!GameDebug.GetVariable("limit_view", true))
                return;

            if (Size.X > GameSettings.Current.MaxWorldView.X || Size.Y > GameSettings.Current.MaxWorldView.Y)
            {
				var zoom = Math.Min(GameSettings.Current.MaxWorldView.Y / Size.Y, GameSettings.Current.MaxWorldView.X / Size.X);
                Scale *= zoom;
            }

            Scale = Math.Max(Scale, _settings.MaxZoomIn);
        }

        public static void Update(float time)
		{
            if (UserSettings.GetBool("Game", "SmoothCam"))
            {
                Center = new Vector2f(
                    Center.X + (TargetPosition.X - Center.X) * time * MOVE_STRENGTH,
                    Center.Y + (TargetPosition.Y - Center.Y) * time * MOVE_STRENGTH
                );
                Scale += (TargetScale - Scale) * time * ZOOM_STRENGTH;
            }
            else
			{
                Center = TargetPosition;
                Scale = TargetScale;
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

        public static View GetWindowView(RenderWindow window)
		{
            return new View(new FloatRect(new Vector2f(0, 0), window.Size.ToVector2f()));
        }
    }
}
