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

namespace TheForestWaiter
{
    class Program
    {
        static readonly StateManager _manager = new StateManager();
        static GameWindow _window;

        static void Main(string[] args)
        {
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

            while (_window.Window.IsOpen)
            {
                timer.Restart();

                _window.Window.DispatchEvents();
                _window.Window.Clear(new Color(46, 36, 115));

                _manager.Update(deltaTime);
                GameDebug.Update(deltaTime);

                _window.Window.SetView(Camera.GetView());
                _manager.Draw(_window.Window);
                GameDebug.Draw(_window.Window);

                _window.Window.SetView(new View(new FloatRect(new Vector2f(0,0), _window.Window.Size.ToVector2f())));
                GameDebug.DrawUI(_window.Window);

                _window.Window.Display();

                deltaTime = (float)timer.Elapsed.TotalSeconds * GameDebug.GetVariable("time_scale", 1f);
            }
        }
    }
}
