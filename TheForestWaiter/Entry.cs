using System.Diagnostics;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.States;

namespace TheForestWaiter
{
	internal class Entry
	{
		private readonly WindowHandle _window;
		private readonly IDebug _debug;
		private readonly StateManager _stateManager;
		private readonly GameState _gameState;
		private readonly TimeProcessor _time;

		public Entry(
			WindowHandle window,
			IDebug debug,
			StateManager stateManager,
			GameState gameState,
			TimeProcessor manager)
		{
			_window = window;
			_debug = debug;
			_stateManager = stateManager;
			_gameState = gameState;
			_time = manager;
		}

		public void Run()
		{
			Stopwatch timer = Stopwatch.StartNew();
			float deltaTime = 0;

			_stateManager.SetState(_gameState);

			while (_window.SfmlWindow.IsOpen)
			{
				timer.Restart();

				_window.SfmlWindow.DispatchEvents();

				_stateManager.Update(deltaTime);
				_debug.Update(deltaTime);

				_stateManager.Draw();
				_debug.Draw(_window.SfmlWindow);

				_window.SfmlWindow.Display();

				deltaTime = _time.Process((float)timer.Elapsed.TotalSeconds);
			}
		}
	}
}
