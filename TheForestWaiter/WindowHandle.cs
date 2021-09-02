using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter
{
    class WindowHandle : ISetup
    {
        private readonly UserSettings _settings;

        public event EventHandler OnWindowChanged = delegate { };

        public RenderWindow SfmlWindow { get; private set; }
        public bool IsFullscreen { get; private set; }

        public WindowHandle(UserSettings settings)
        {
            _settings = settings;
        }

        public void InitializeWindow(bool fullscreen)
        {
            if (SfmlWindow != null)
                SfmlWindow.Close();

            var title = _settings.Get("Window", "Title");  //TODO: make this type safe (remove the string methods from settings)
            var maxFps = (uint)_settings.GetInt("Window", "MaxFramerate");

            if (fullscreen)
            {
                SfmlWindow = new RenderWindow(VideoMode.DesktopMode, title, Styles.Fullscreen);
                IsFullscreen = true;
            }
            else
            {
                var width = (uint)_settings.GetInt("Window", "Width");
                var height = (uint)_settings.GetInt("Window", "Height");

                SfmlWindow = new RenderWindow(new VideoMode(width, height), title, Styles.Close | Styles.Resize);
                IsFullscreen = false;
            }

            SfmlWindow.SetFramerateLimit(maxFps);

            OnWindowChanged(this, EventArgs.Empty);
        }

        public void Setup()
        {
            InitializeWindow(false);
            SfmlWindow.SetKeyRepeatEnabled(false);
        }
    }
}
