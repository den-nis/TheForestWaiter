using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Essentials;
using LightInject;
using TheForestWaiter.Services;
using TheForestWaiter.States;

namespace TheForestWaiter.Game
{
    class GameState : IState, IDisposable
    {
        private readonly GameServices _services;

        private readonly WindowHandle _window;
        private readonly Camera _camera;
        private readonly GameContent _content;

        private readonly GameData _game;

        private const float CLEAN_UP_INTERVAL = 10;
        private float _cleanUpTimer = CLEAN_UP_INTERVAL;
        private readonly Background _background;

        public GameState(WindowHandle window, GameContent content, ServiceContainer container)
        {
            _services = new GameServices(container);
            _services.Register();

            _game = container.GetInstance<GameData>();
            _camera = container.GetInstance<Camera>();
            _background = container.GetInstance<Background>();
            _window = window;
            _content = content;
        }


        public void Dispose()
        {
            _services.Dispose();
        }

        public void Load()
        {
            _services.Setup();

            var map = LoadMap();
            _game.LoadFromMap(map);
            SetupCamera();

            _background.Horizon = _game.Objects.Player.Position.Y;
        }

        private Map LoadMap()
        {
            return JsonConvert.DeserializeObject<Map>(_content.Source.GetString("Worlds\\main.json"));
        }

        private void SetupCamera()
        {
            _camera.BaseSize = _window.SfmlWindow.Size.ToVector2f();
            _camera.Center = _game.Objects.Player.Center;
        }

        public void Draw()
        {
            _window.SfmlWindow.SetView(_camera.GetView());
            _window.SfmlWindow.Clear(new Color(54, 26, 103));

            _background.Draw(_window.SfmlWindow);

            _game.World.Draw(_window.SfmlWindow, new FloatRect(_camera.Position, _camera.Size), false);
            _game.Objects.Draw(_window.SfmlWindow);
            _game.World.Draw(_window.SfmlWindow, new FloatRect(_camera.Position, _camera.Size), true);

        }

        public void Update(float time)
        {
            _camera.Update(time);
            _background.Update();
            _background.SetOffset(_camera.Center);
            _background.UpdateSize((int)_window.SfmlWindow.Size.X, (int)_window.SfmlWindow.Size.Y);

            _cleanUpTimer -= time;
            if (_cleanUpTimer < 0)
            {
                _cleanUpTimer = CLEAN_UP_INTERVAL;
                _game.Objects.CleanUp();
            }

            _camera.TargetPosition = _game.Objects.Player.Center;
            _game.Objects.Update(time);
        }
    }
}
