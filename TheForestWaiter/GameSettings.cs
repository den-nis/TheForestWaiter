using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
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

        public float MaxZoomIn { get; } = 0.09f;
        public float MaxZoomOut { get; } = 1000.00f;

        public int MaxWorldParticles { get; } = 50000;
        public bool DisabledParticles { get; } = false;

        public bool ShowFramerate { get; } = true;

        public TimeSpan EarlyJumpTime { get; } = TimeSpan.FromMilliseconds(100);

    }
}
