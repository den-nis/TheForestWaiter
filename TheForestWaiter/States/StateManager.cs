using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.States
{
	internal class StateManager
	{
		public bool IsTransitioning => _transitionQueue.Any();
		public IState CurrentState { get; set; }

		private readonly Queue<StateTransition> _transitionQueue = new();
		private float _transitionTimer = 0;
		private bool _transitionHasChangedState = false;

		private readonly RectangleShape _overlay = new();
		private readonly WindowHandle _window;

		public StateManager()
		{
			_window = IoC.GetInstance<WindowHandle>();
			_overlay.FillColor = Color.Transparent;
		}

		public void SetState<T>() where T : IState, new() => SetState(new T());

		public void SetState(IState state)
		{
			CurrentState?.Dispose();
			state.Load();
			CurrentState = state;
		}

		public void StartTransition(StateTransition transition, bool force = false)
		{
			if (force || !_transitionQueue.Any())
			{
				Debug.WriteLine("Starting transition");
				_transitionQueue.Enqueue(transition);
			}
			else
			{
				Debug.WriteLine("Statemanager is busy. Refused to start state");
			}
		}

		public void Draw()
		{
			CurrentState.Draw();
			_window.SfmlWindow.Draw(_overlay);
		}

		public void Update(float time)
		{
			if (_transitionQueue.Any())
			{
				_transitionTimer += time;

				var activeTransition = _transitionQueue.Peek();
				var delta = _transitionTimer / activeTransition.Length;

				if (delta < 1)
				{
					var animation = delta > 0.5 ? (1 - delta) * 2 : delta * 2;
					var color = new Color(activeTransition.Color)
					{
						A = (byte)(animation * 255)
					};

					_overlay.FillColor = color;
					_overlay.Size = _window.SfmlWindow.Size.ToVector2f();

					if (!_transitionHasChangedState && delta >= .5f)
					{
						var state = (IState)Activator.CreateInstance(activeTransition.TargetState);
						SetState(state);
						_transitionHasChangedState = true;
					}
				}
				else
				{
					_transitionQueue.Dequeue();
					_transitionHasChangedState = false;
					_transitionTimer = 0;
				}
			}

			CurrentState.Update(time);
		}
	}
}
