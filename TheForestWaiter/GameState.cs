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
        private GameWindow _gameWindow;
        private GameControler Controler { get; }
        private GameData Data { get; } = new GameData();
        private const float CLEAN_UP_INTERVAL = 10;
        private float CleanUpTimer { get; set; } = CLEAN_UP_INTERVAL;

        public GameState(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            Controler = new GameControler(Data, gameWindow);
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
            Data.World.Draw(_gameWindow.Window, new FloatRect(Camera.Position, Camera.Size), false);
            Data.Objects.Draw(_gameWindow.Window);
            Data.World.Draw(_gameWindow.Window, new FloatRect(Camera.Position, Camera.Size), true);
        }

        public void Update(float time)
        {
            CleanUpTimer -= time;
            if (CleanUpTimer < 0)
            {
                CleanUpTimer = CLEAN_UP_INTERVAL;
                Data.Objects.CleanUp();
            }

            Camera.Center = Data.Objects.Player.Center;
            Data.Objects.Update(time);
        }
    }
}
