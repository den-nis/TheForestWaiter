using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game
{
    class GameController : IDisposable
    {
        private RenderWindow _attached;
        private readonly GameData _data;
        private readonly WindowHandle _gameWindow;
        private readonly UserSettings _settings;
        private readonly Camera _camera;

        public GameController(
            GameData data, 
            WindowHandle gameWindow, 
            UserSettings settings,
            Camera camera)
        {
            _data = data;
            _gameWindow = gameWindow;
            _settings = settings;
            _camera = camera;
            _gameWindow.OnWindowChanged += OnWindowChanged;

            Attach(_gameWindow.SfmlWindow);
        }

        private void OnWindowChanged(object sender, EventArgs e)
        {
            Attach(_gameWindow.SfmlWindow);
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
            _gameWindow.SfmlWindow.Close();
        }

        private void WindowResized(object sender, SizeEventArgs e)
        {
            _camera.BaseSize = new Vector2f(e.Width, e.Height);
        }

        private void ToggleFullscreen()
        {
            _gameWindow.InitializeWindow(!_gameWindow.IsFullscreen);
            _camera.BaseSize = _gameWindow.SfmlWindow.Size.ToVector2f();
        }

        private void WindowMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == _settings.Shoot)
                _data.Objects.Player.Controller.ToggleOff(ActionTypes.PrimaryAttack);
        }

        private void WindowMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == _settings.Shoot)
                _data.Objects.Player.Controller.ToggleOn(ActionTypes.PrimaryAttack);
        }

        private void WindowMouseMoved(object sender, MouseMoveEventArgs e)
        {
            var mouse = new Vector2f(e.X, e.Y);
            var angle = (_camera.ToWorld(mouse) - _data.Objects.Player.Center).Angle() + (float)Math.PI * 2;
            _data.Objects.Player.Controller.Aim(angle);
        }

        private void WindowMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            _camera.TargetScale = _camera.Scale - (_camera.Scale * (e.Delta / 3));
        }

        private void WindowKeyReleased(object sender, KeyEventArgs e)
        {
            var c = e.Code;
            if (c == _settings.Jump)  _data.Objects.Player.Controller.ToggleOff(ActionTypes.Up);
            if (c == _settings.Right) _data.Objects.Player.Controller.ToggleOff(ActionTypes.Right);
            if (c == _settings.Left)  _data.Objects.Player.Controller.ToggleOff(ActionTypes.Left);
            if (c == _settings.FullScreen) ToggleFullscreen();
        }

        private void WindowKeyPressed(object sender, KeyEventArgs e)
        {
            var c = e.Code;
            if (c == _settings.Jump)  _data.Objects.Player.Controller.ToggleOn(ActionTypes.Up);
            if (c == _settings.Right) _data.Objects.Player.Controller.ToggleOn(ActionTypes.Right);
            if (c == _settings.Left)  _data.Objects.Player.Controller.ToggleOn(ActionTypes.Left);
        }

        public void Dispose()
        {
            _gameWindow.OnWindowChanged -= OnWindowChanged;
            Detach();
        }
    }
}
