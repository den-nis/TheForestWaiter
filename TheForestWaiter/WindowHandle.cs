using SFML.Graphics;
using SFML.Window;
using System;
using TheForestWaiter.Content;

namespace TheForestWaiter
{
	internal class WindowHandle : ISetup
	{
		public event EventHandler OnWindowChanged = delegate { };

		public RenderWindow SfmlWindow { get; private set; }
		public bool IsFullscreen { get; private set; }

		private readonly ContentSource _content;
		private readonly UserSettings _settings;

		public WindowHandle()
		{
			_content = IoC.GetInstance<ContentSource>();
			_settings = IoC.GetInstance<UserSettings>();
		}

		public void InitializeWindow(bool fullscreen)
		{
			if (SfmlWindow != null)
				SfmlWindow.Close();

			var title = _settings.Get("Window", "Title");
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

			using var image = new Image(_content.Source.GetBytes("Textures/Icon.png"));
			SfmlWindow.SetIcon(256, 256, image.Pixels);

			OnWindowChanged(this, EventArgs.Empty);
		}

		public void Setup()
		{
			InitializeWindow(false);
			SfmlWindow.SetKeyRepeatEnabled(false);
		}
	}
}
