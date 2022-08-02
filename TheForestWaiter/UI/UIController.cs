using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;

namespace TheForestWaiter.Game
{
	sealed internal class UIController : IDisposable
	{
		private RenderWindow _attached;
		private readonly WindowHandle _gameWindow;
		private readonly UserSettings _settings;

		public event Action OnMousePressed;
		public event Action OnMouseReleased;
		public event Action<Vector2f> OnMouseMove;

		public UIController()
		{
			_gameWindow = IoC.GetInstance<WindowHandle>();
			_settings = IoC.GetInstance<UserSettings>();
		}

		public void Setup()
		{
			_gameWindow.OnWindowChanged += OnWindowChanged;
			Attach(_gameWindow.SfmlWindow);
		}

		private void OnWindowChanged(object sender, EventArgs e)
		{
			Attach(_gameWindow.SfmlWindow);
		}

		private void Attach(RenderWindow window)
		{
			Debug.WriteLine("Attaching UI controller");

			if (_attached != null)
				Detach();
			_attached = window;

			window.KeyReleased += WindowKeyReleased;
			window.MouseMoved += WindowMouseMoved;
			window.MouseButtonPressed += WindowMouseButtonPressed;
			window.MouseButtonReleased += WindowMouseButtonReleased;
			window.Closed += WindowClosed;
		}

		private void Detach()
		{
			Debug.WriteLine("Detaching UI controller");

			_attached.KeyReleased -= WindowKeyReleased;
			_attached.MouseMoved -= WindowMouseMoved;
			_attached.MouseButtonPressed -= WindowMouseButtonPressed;
			_attached.MouseButtonReleased -= WindowMouseButtonReleased;
			_attached.Closed += WindowClosed;
		}

		private void WindowClosed(object sender, EventArgs e)
		{
			_gameWindow.SfmlWindow.Close();
		}

		private void ToggleFullscreen()
		{
			_gameWindow.InitializeWindow(!_gameWindow.IsFullscreen);
		}

		private void WindowMouseButtonReleased(object sender, MouseButtonEventArgs e)
		{
			OnMouseReleased?.Invoke();
		}

		private void WindowMouseButtonPressed(object sender, MouseButtonEventArgs e)
		{
			OnMousePressed?.Invoke();
		}

		private void WindowMouseMoved(object sender, MouseMoveEventArgs e)
		{
			OnMouseMove?.Invoke(new Vector2f(e.X, e.Y));
		}

		private void WindowKeyReleased(object sender, KeyEventArgs e)
		{
			var c = e.Code;
			if (c == _settings.FullScreen) ToggleFullscreen();
		}

		public void Dispose()
		{
			_gameWindow.OnWindowChanged -= OnWindowChanged;
			Detach();
		}
	}
}
