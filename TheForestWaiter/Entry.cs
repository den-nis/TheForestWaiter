﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.States;

namespace TheForestWaiter
{
    class Entry
    {
        public bool LockDelta { get; set; } = false;
        public bool LagLimit { get; set; } = true;
        public float TimeScale { get; set; } = 1; 

        private readonly WindowHandle _window;
        private readonly IGameDebug _debug;
        private readonly StateManager _stateManager;
        private readonly GameState _gameState;

        public Entry(
            WindowHandle window, 
            IGameDebug debug, 
            StateManager stateManager,
            GameState gameState)
        {
            _window = window;
            _debug = debug;
            _stateManager = stateManager;
            _gameState = gameState;
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

                deltaTime = (float)timer.Elapsed.TotalSeconds * TimeScale;

                if (LockDelta)
                    deltaTime = 0.01f;

                if (LagLimit)
                    deltaTime = Math.Min(deltaTime, 1);
            }
        }
    }
}