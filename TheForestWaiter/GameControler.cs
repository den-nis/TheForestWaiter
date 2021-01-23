using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
    class GameControler
    {
        private GameData Game { get; }
        private RenderWindow Window { get; }

        public GameControler(RenderWindow window, GameData game)
        {
            Game = game;
            Window = window;

            Window.KeyPressed += WindowKeyPressed;
            Window.KeyReleased += WindowKeyReleased;
            Window.MouseWheelScrolled += WindowMouseWheelScrolled;
            Window.MouseMoved += WindowMouseMoved;
            Window.MouseButtonPressed += WindowMouseButtonPressed;
            Window.MouseButtonReleased += WindowMouseButtonReleased;
            window.Resized += WindowResized;
        }

        private void WindowResized(object sender, SizeEventArgs e)
        {
            Camera.BaseSize = new Vector2f(e.Width, e.Height);
        }

        private void WindowMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
                Game.Objects.Player.StopFire();
        }

        private void WindowMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
                Game.Objects.Player.StartFire();
        }

        private void WindowMouseMoved(object sender, MouseMoveEventArgs e)
        {
            Game.Objects.Player.Aim(new Vector2f(e.X, e.Y));
        }

        private void WindowMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            var settings = GameSettings.Current;

            Camera.Scale = (float)Math.Round(Camera.Scale - (e.Delta/10), 1);
            Camera.Scale = Math.Max(Camera.Scale, settings.MaxZoomIn);
        }

        private void WindowKeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Space: Game.Objects.Player.StopJump(); break;
                case Keyboard.Key.D: Game.Objects.Player.StopMoveRight(); break;
                case Keyboard.Key.A: Game.Objects.Player.StopMoveLeft(); break;
            }
        }

        private void WindowKeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Space: Game.Objects.Player.StartJump(); break;
                case Keyboard.Key.D: Game.Objects.Player.StartMoveRight(); break;
                case Keyboard.Key.A: Game.Objects.Player.StartMoveLeft(); break;
            }
        }

        public void UnsubscribeEvents()
        {
            Window.KeyPressed -= WindowKeyPressed;
            Window.KeyReleased -= WindowKeyReleased;
            Window.MouseWheelScrolled -= WindowMouseWheelScrolled;
        }
    }
}
