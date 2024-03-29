﻿using SFML.Graphics;
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
		private float _delayTimer = 0;
		private bool _transitionHasChangedState = false;

		private readonly RectangleShape _overlay = new();
		private readonly WindowHandle _window;

		public StateManager()
		{
			_window = IoC.GetInstance<WindowHandle>();
			_overlay.FillColor = Color.Transparent;
		}

		public void ForceClearQueue()
		{
			_transitionQueue.Clear();
			Reset();
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
			UpdateLogic(time);
			CurrentState.Update(time);
		}

		private void UpdateLogic(float time)
		{
			_overlay.Size = _window.SfmlWindow.Size.ToVector2f();

			if (!_transitionQueue.Any())
				return;

			var activeTransition = _transitionQueue.Peek();

			if (activeTransition.Delay > _delayTimer)
			{
				_delayTimer += time;
				return;
			}

			_transitionTimer += time;
			var delta = _transitionTimer / activeTransition.Length;

			if (delta < 1)
			{
				var animation = delta > 0.5 ? (1 - delta) * 2 : delta * 2;

				SetOverlayColor((byte)(animation * 255), activeTransition.Color);

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
				Reset();
			}
		}

		private void SetOverlayColor(byte alpha, Color color)
		{
			_overlay.FillColor = new Color(color)
			{
				A = alpha
			};
		}

		private void Reset()
		{
			_transitionHasChangedState = false;
			_transitionTimer = _delayTimer = 0;
			SetOverlayColor(0, Color.Black);
		}
	}
}
