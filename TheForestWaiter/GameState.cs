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
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Objects;
using TheForestWaiter.States;

namespace TheForestWaiter
{
    class GameState : IState, IDisposable
    {
        private readonly GameWindow _gameWindow;
        private GameControler Controler { get; }
        private GameData Data { get; } = new GameData();
        private const float CLEAN_UP_INTERVAL = 10;
        private float _cleanUpTimer = CLEAN_UP_INTERVAL;
        private readonly Background _background;

        public GameState(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            Controler = new GameControler(Data, _gameWindow);
            _background = new Background((int)GameSettings.Current.MaxWorldView.X, (int)GameSettings.Current.MaxWorldView.Y);
        }

        public void Dispose()
        {
            Controler.Dispose();
        }

        public void Load()
        {
            var map = LoadMap();
            Data.LoadFromMap(map);
            SetupCamera();

            _background.Horizon = Data.Objects.Player.Position.Y;

            GameDebug.ProvideGameData(Data);
        }

        private static Map LoadMap()
        {
            return JsonConvert.DeserializeObject<Map>(GameContent.Source.GetString("Worlds\\main.json"));
        }

        private void SetupCamera()
        {
            Camera.BaseSize = _gameWindow.Window.Size.ToVector2f();
            Camera.Center = Data.Objects.Player.Center;
        }

        public void Draw()
        {
            _gameWindow.Window.Clear(new Color(54, 26, 103));

            _background.Draw(_gameWindow.Window);

            Data.World.Draw(_gameWindow.Window, new FloatRect(Camera.Position, Camera.Size), false);
            Data.Objects.Draw(_gameWindow.Window);
            Data.World.Draw(_gameWindow.Window, new FloatRect(Camera.Position, Camera.Size), true);

        }

        public void Update(float time)
        {
            _background.Update();
            _background.SetOffset(Data.Objects.Player.Position);
            _background.UpdateSize((int)_gameWindow.Window.Size.X, (int)_gameWindow.Window.Size.Y);

            _cleanUpTimer -= time;
            if (_cleanUpTimer < 0)
            {
                _cleanUpTimer = CLEAN_UP_INTERVAL;
                Data.Objects.CleanUp();
            }

            Camera.Center = Data.Objects.Player.Center;
            Data.Objects.Update(time);
        }
	}
}
