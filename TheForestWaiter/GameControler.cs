using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Essentials;

namespace TheForestWaiter
{
    class GameControler : IDisposable
    {
        private RenderWindow _attached;
        private readonly GameData _data;
        private readonly GameWindow _gameWindow;

        public GameControler(GameData data, GameWindow gameWindow)
        {
            _data = data;
            _gameWindow = gameWindow;
            _gameWindow.OnWindowChanged += OnWindowChanged;
            Attach(_gameWindow.Window);
        }

        private void OnWindowChanged(object sender, EventArgs e)
        {
            Attach(_gameWindow.Window);
        }

        private void Attach(RenderWindow window)
        {
            if (_attached != null)
                Detach();
            _attached = window;

            window.KeyPressed += WindowKeyPressed;
            window.KeyReleased += WindowKeyReleased;
            window.MouseWheelScrolled += WindowMouseWheelScrolled;
            window.MouseMoved += WindowMouseMoved;
            window.MouseButtonPressed += WindowMouseButtonPressed;
            window.MouseButtonReleased += WindowMouseButtonReleased;
            window.Resized += WindowResized;
            window.Closed += WindowClosed;
        }

        private void Detach()
        {
            _attached.KeyPressed -= WindowKeyPressed;
            _attached.KeyReleased -= WindowKeyReleased;
            _attached.MouseWheelScrolled -= WindowMouseWheelScrolled;
            _attached.MouseMoved -= WindowMouseMoved;
            _attached.MouseButtonPressed -= WindowMouseButtonPressed;
            _attached.MouseButtonReleased -= WindowMouseButtonReleased;
            _attached.Resized -= WindowResized;
            _attached.Closed += WindowClosed;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            _gameWindow.Window.Close();
        }

        private void WindowResized(object sender, SizeEventArgs e)
        {
            Camera.BaseSize = new Vector2f(e.Width, e.Height);
        }

        private void ToggleFullscreen()
        {
            _gameWindow.InitializeWindow(!_gameWindow.IsFullscreen);
            Camera.BaseSize = _gameWindow.Window.Size.ToVector2f();
        }

        private void WindowMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == UserSettings.Shoot)
                _data.Objects.Player.StopFire();
        }

        private void WindowMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == UserSettings.Shoot)
                _data.Objects.Player.StartFire();
        }

        private void WindowMouseMoved(object sender, MouseMoveEventArgs e)
        {
            _data.Objects.Player.Aim(new Vector2f(e.X, e.Y));
        }

        private void WindowMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            var settings = GameSettings.Current;

            Camera.Scale = (float)Math.Round(Camera.Scale - (e.Delta/10), 1);
            Camera.Scale = Math.Max(Camera.Scale, settings.MaxZoomIn);
        }

        private void WindowKeyReleased(object sender, KeyEventArgs e)
        {
            var c = e.Code;
            if (c == UserSettings.Jump)  _data.Objects.Player.StopJump();
            if (c == UserSettings.Right) _data.Objects.Player.StopMoveRight();
            if (c == UserSettings.Left)  _data.Objects.Player.StopMoveLeft();
            if (c == UserSettings.Fullscreen) ToggleFullscreen();
        }

        private void WindowKeyPressed(object sender, KeyEventArgs e)
        {
            var c = e.Code;
            if (c == UserSettings.Jump)  _data.Objects.Player.StartJump();
            if (c == UserSettings.Right) _data.Objects.Player.StartMoveRight();
            if (c == UserSettings.Left)  _data.Objects.Player.StartMoveLeft();
        }

        public void Dispose()
        {
            _gameWindow.OnWindowChanged -= OnWindowChanged;
            Detach();
        }
    }
}
