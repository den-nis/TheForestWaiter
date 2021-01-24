using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter
{
    class GameWindow
    {
        private const int DEFAULT_MINIMIZED_WIDTH = 800;
        private const int DEFAULT_MINIMIZED_HEIGHT = 600;
        private const string TITLE = "The forest waiter";

        public event EventHandler OnWindowChanged = delegate { };

        public RenderWindow Window { get; private set; }
        public bool IsFullscreen { get; private set; }

        public void InitializeWindow(bool fullscreen)
        {
            if (Window != null)
                Window.Close();

            if (fullscreen)
            {
                Window = new RenderWindow(VideoMode.DesktopMode, TITLE, Styles.Fullscreen);
                IsFullscreen = true;
            }
            else
            {
                Window = new RenderWindow(new VideoMode(DEFAULT_MINIMIZED_WIDTH, DEFAULT_MINIMIZED_HEIGHT), TITLE, Styles.Close | Styles.Resize);
                IsFullscreen = false;
            }

            Window.SetFramerateLimit(150);

            OnWindowChanged(this, EventArgs.Empty);
        }
    }
}
