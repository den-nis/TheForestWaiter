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
using TheForestWaiter.Debugging;
using TheForestWaiter.Environment;
using TheForestWaiter.Objects;
using TheForestWaiter.Resources;
using TheForestWaiter.States;

namespace TheForestWaiter
{
    class GameState : IState, IDisposable
    {
        private RenderWindow Window { get; }
        private GameControler Controler { get; }
        private GameData Data { get; } = new GameData();
        private const float CLEAN_UP_INTERVAL = 10;
        private float CleanUpTimer { get; set; } = CLEAN_UP_INTERVAL;

        public GameState(RenderWindow window)
        {
            Window = window;
            Controler = new GameControler(Window, Data);
        }

        public void Dispose()
        {
            Controler.UnsubscribeEvents();
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
            using StreamReader reader = new StreamReader(GameContent.LoadContentStream(@"Content.Worlds.main.json"));
            return JsonConvert.DeserializeObject<Map>(reader.ReadToEnd());
        }

        private void SetupCamera()
        {
            var settings = GameSettings.Current;
            Camera.BaseSize = new Vector2f(settings.WindowWidth, settings.WindowHeight);
            Camera.Center = Data.Objects.Player.Center;
        }

        public void Draw()
        {    
            Data.World.Draw(Window, new FloatRect(Camera.Position, Camera.Size), TileLayers.Middleground | TileLayers.Background);
            Data.Objects.Draw(Window);
            Data.World.Draw(Window, new FloatRect(Camera.Position, Camera.Size), TileLayers.Foreground);
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
