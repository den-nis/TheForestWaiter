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
        public event EventHandler OnWindowChanged = delegate { };

        public RenderWindow Window { get; private set; }
        public bool IsFullscreen { get; private set; }

        public void InitializeWindow(bool fullscreen)
        {
            if (Window != null)
                Window.Close();

            var title = UserSettings.Get("Window", "Title");
            var maxFps = uint.Parse(UserSettings.Get("Window", "MaxFramerate"));

            if (fullscreen)
            {
                Window = new RenderWindow(VideoMode.DesktopMode, title, Styles.Fullscreen);
                IsFullscreen = true;
            }
            else
            {
                var width = uint.Parse(UserSettings.Get("Window", "Width"));
                var height = uint.Parse(UserSettings.Get("Window", "Height"));

                Window = new RenderWindow(new VideoMode(width, height), title, Styles.Close | Styles.Resize);
                IsFullscreen = false;
            }

            Window.SetFramerateLimit(maxFps);

            OnWindowChanged(this, EventArgs.Empty);
        }
    }
}
