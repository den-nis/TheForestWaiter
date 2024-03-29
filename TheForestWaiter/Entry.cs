﻿using System.Diagnostics;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Performance;
using TheForestWaiter.States;
using TheForestWaiter.UI.Menus;

namespace TheForestWaiter
{
	internal class Entry
	{
		private readonly WindowHandle _window;
		private readonly IDebug _debug;
		private readonly StateManager _stateManager;
		private readonly MainMenu _menu;
		private readonly TimeProcessor _time;

		public Entry(
			WindowHandle window,
			IDebug debug,
			StateManager stateManager,
			MainMenu menu,
			TimeProcessor manager)
		{
			_window = window;
			_debug = debug;
			_stateManager = stateManager;
			_menu = menu;
			_time = manager;
		}

		public void Run()
		{
			Profiling.CreateProfiler();

			Stopwatch timer = Stopwatch.StartNew();
			float deltaTime = 0;

			_stateManager.SetState(_menu);

			while (_window.SfmlWindow.IsOpen)
			{
				Profiling.Start(ProfileCategory.Tick);

				timer.Restart();

				_window.SfmlWindow.DispatchEvents();

				_stateManager.Update(deltaTime);
				_debug.Update(deltaTime);

				_stateManager.Draw();
				_debug.Draw(_window.SfmlWindow);

				Profiling.End(ProfileCategory.Tick);

				_window.SfmlWindow.Display();

				deltaTime = _time.Process((float)timer.Elapsed.TotalSeconds);

				Profiling.SendDataToProfiler();
			}
		}
	}
}
