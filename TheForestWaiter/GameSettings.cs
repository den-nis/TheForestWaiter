using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
    //TODO: can this whole class be removed?
    class GameSettings
    {
        //TODO: load from json?
        public static GameSettings Current => new GameSettings();

        private GameSettings()
        {

        }

        public string Title { get; } = "The forest waiter";

        public int WindowWidth { get; } = 800;
        public int WindowHeight { get; } = 600;

        public float MaxZoomIn { get; } = 0.1f;

        public Vector2f MaxWorldView { get; } = new Vector2f(1920, 1080);

        public int MaxWorldParticles { get; } = 50000;
        public bool DisabledParticles { get; } = false;

        public bool ShowFramerate { get; } = true;
    }
}
