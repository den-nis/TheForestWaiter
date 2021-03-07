using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TheForestWaiter.Objects;
using System.Linq;
using TheForestWaiter.Environment;
using TheForestWaiter.States;
using TheForestWaiter.Debugging;
using TheForestWaiter.Content;
using TheForestWaiter.Essentials;
using System.Globalization;

namespace TheForestWaiter
{
    class Program
    {
        static readonly StateManager _manager = new StateManager();
        static GameWindow _window;

        static void Main()
        {
            try
            {
                Run();
            }
            catch(Exception e)
			{
                Crash.Now(e);
			}
        }

        static void Run()
        {
            SetEnglishCultureInfo();
            Stopwatch startupTimer = Stopwatch.StartNew();

            UserSettings.Load();

            _window = new GameWindow();
            _window.InitializeWindow(false);
            _window.Window.SetKeyRepeatEnabled(false);

            GameContent.Initialize();
            _manager.SetState(new GameState(_window));

            GameDebug.ProvideWindow(_window);
            GameDebug.StartConsoleThread();

            Stopwatch timer = Stopwatch.StartNew();
            float deltaTime = 0;

            GameDebug.Log($"Finished startup in {startupTimer.Elapsed.TotalSeconds} seconds");

            while (_window.Window.IsOpen)
            {
                timer.Restart();

                _window.Window.DispatchEvents();

                _manager.Update(deltaTime);
                GameDebug.Update(deltaTime);

                _window.Window.SetView(Camera.GetView());
                _manager.Draw();
                GameDebug.Draw(_window.Window);

                _window.Window.Display();

                deltaTime = (float)timer.Elapsed.TotalSeconds * GameDebug.GetVariable("time_scale", 1f);

                if (GameDebug.GetVariable("lag_limit", true))
                    deltaTime = Math.Min(deltaTime, 1);
            }
        }


        static void SetEnglishCultureInfo()
		{
            var cInfo = new CultureInfo("en");
            CultureInfo.DefaultThreadCurrentCulture = cInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cInfo;
            CultureInfo.CurrentCulture = cInfo;
        }
    }
}
