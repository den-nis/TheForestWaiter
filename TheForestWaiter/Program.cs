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
        static void Main(string[] args)
        {
            GameWindow gw = new GameWindow();
            gw.InitializeWindow(false);
            gw.Window.SetKeyRepeatEnabled(false);

            GameContent.Initialize();
            StateManager manager = new StateManager();
            manager.SetState(new GameState(gw));
            GameDebug.StartConsoleThread();

            Stopwatch timer = Stopwatch.StartNew();
            float deltaTime = 0;
            while (gw.Window.IsOpen)
            {
                timer.Restart();

                gw.Window.DispatchEvents();
                gw.Window.Clear(Color.Black);//new Color(46,36,115));

                manager.Update(deltaTime);
                GameDebug.Update(deltaTime);

                gw.Window.SetView(Camera.GetView());
                manager.Draw(gw.Window);
                GameDebug.Draw(gw.Window);

                gw.Window.SetView(new View(new FloatRect(new Vector2f(0,0), gw.Window.Size.ToVector2f())));
                GameDebug.DrawUI(gw.Window);

                gw.Window.Display();

                deltaTime = Math.Min(0.1f, (float)timer.Elapsed.TotalSeconds * GameDebug.GetVariable<float>("time_scale", 1));
            }
        }
    }
}
