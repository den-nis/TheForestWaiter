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

namespace TheForestWaiter
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = GameSettings.Current;
            RenderWindow window = new RenderWindow(new VideoMode((uint)settings.WindowWidth, (uint)settings.WindowHeight), settings.Title, Styles.Titlebar | Styles.Close);
            window.Closed += (x,y) => window.Close();

            window.SetKeyRepeatEnabled(false);
            window.Clear(Color.Black); window.Display();

            GameContent.LoadAllContent();

            StateManager manager = new StateManager();
            manager.SetState(new GameState(window));
            GameDebug.StartConsoleThread();
            Stopwatch timer = Stopwatch.StartNew();

            float deltaTime = 0;
            while (window.IsOpen)
            {
                timer.Restart();

                window.DispatchEvents();
                window.Clear(Color.Black);//new Color(46,36,115));

                manager.Update(deltaTime);
                GameDebug.Update(deltaTime);

                window.SetView(Camera.GetView());
                manager.Draw(window);
                GameDebug.Draw(window);

                window.SetView(new View(new FloatRect(0, 0, settings.WindowWidth, settings.WindowHeight)));
                GameDebug.DrawUI(window);

                window.Display();

                deltaTime = Math.Min(0.1f, (float)timer.Elapsed.TotalSeconds * GameDebug.GetVariable<float>("time_scale", 1));
            }
        }
    }
}
