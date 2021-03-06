using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
    class GameSettings
    {
        public static GameSettings Current => new GameSettings();

        public float MaxZoomIn { get; } = 0.1f;
        public Vector2f MaxWorldView { get; } = new Vector2f(1920, 1080);
    }
}
