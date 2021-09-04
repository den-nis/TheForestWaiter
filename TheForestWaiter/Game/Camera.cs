using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game
{
    class Camera
    {
        private const float MAX_ZOOM_IN = 0.1f;
        private const float ZOOM_STRENGTH = 10;

        private const float HORIZONTAL_MOVE_STRENGTH = 100;
        private const float VERTICAL_MOVE_STRENGTH = 20;

        public Vector2f MaxWorldView { get; } = new Vector2f(1920, 1080);
        public Vector2f TargetPosition { get; set; }
        public float TargetScale { get; set; } = 1;

        private Vector2f _baseSize = default;
        public Vector2f BaseSize 
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

        private float _scale = 1;
        public float Scale 
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

        public Vector2f Size => BaseSize * Scale;

        public Vector2f Position {
            get => Center - Size / 2;
            set => Center = value + Size / 2;
        }

        public Vector2f Center { get; set; }

        public bool LockView { get; set; } = true;

        private readonly UserSettings _settings;

        public Camera(UserSettings settings)
        {
            _settings = settings;
        }

        private void SizeChanged()
        {
            if (!LockView)
                return;

            if (Size.X > MaxWorldView.X || Size.Y > MaxWorldView.Y)
            {
				var zoom = Math.Min(MaxWorldView.Y / Size.Y, MaxWorldView.X / Size.X);
                Scale *= zoom;
            }

            Scale = Math.Max(Scale, MAX_ZOOM_IN);
        }

        public void Update(float time)
		{
            if (_settings.GetBool("Game", "SmoothCam")) //TODO: type safe
            {
                float xDelta = TargetPosition.X - Center.X;
                float yDelta = TargetPosition.Y - Center.Y;
                float sDelta = TargetScale - Scale;

                Center = new Vector2f(
                    Center.X + xDelta * (time * HORIZONTAL_MOVE_STRENGTH > 1 ? 1 : time * HORIZONTAL_MOVE_STRENGTH),
                    Center.Y + yDelta * (time * VERTICAL_MOVE_STRENGTH > 1 ? 1 : time * VERTICAL_MOVE_STRENGTH)
                );
                Scale += sDelta * (time * ZOOM_STRENGTH > 1 ? 1 : time * ZOOM_STRENGTH);
            }
            else
			{
                Center = TargetPosition;
                Scale = TargetScale;
			}
        }

        public Vector2f ToWorld(Vector2f view)
        {
            return Position + (view * Scale);
        }

        public Vector2f ToCamera(Vector2f world)
        {
            return (world - Position) / Scale;
        }

		public View GetView()
		{
			return new View(Center, Size);
		}

        public static View GetWindowView(RenderWindow window)
		{
            return new View(new FloatRect(new Vector2f(0, 0), window.Size.ToVector2f()));
        }
    }
}
