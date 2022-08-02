using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TheForestWaiter.Content;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Services;
using TheForestWaiter.States;

namespace TheForestWaiter.UI.Abstract
{
	internal abstract class UIState : IState
	{
		protected List<Control> Controls { get; } = new();
		protected readonly WindowHandle Window;
		private readonly Sprite _background;
		private readonly UIServices _services;
		private readonly UIController _controller;

		public UIState()
		{
			_services = new UIServices();
			_services.Register();

			var content = IoC.GetInstance<ContentSource>();
			_controller = IoC.GetInstance<UIController>();
			Window = IoC.GetInstance<WindowHandle>();

			_controller.OnMousePressed += OnMousePressed;
			_controller.OnMouseReleased += OnMouseReleased;
			_controller.OnMouseMove += OnMouseMove;

			_background = content.Textures.CreateSprite("Textures/Menu/background.png");
		}

		public virtual void Draw()
		{
			var center = Window.SfmlWindow.Size / 2;
			Window.SfmlWindow.SetView(new View(center.ToVector2f(), Window.SfmlWindow.Size.ToVector2f()));

			Window.SfmlWindow.Clear(new Color(255, 192, 26));
			Window.SfmlWindow.Draw(_background);

			foreach (var control in Controls)
			{
				control.Draw(Window.SfmlWindow);
			}

			Window.SfmlWindow.SetView(Camera.GetWindowView(Window.SfmlWindow));
		}

		public void Update(float time)
		{
			ScaleBackground();

			foreach (var control in Controls)
			{
				control.Update(time);
			}
		}

		private void ScaleBackground()
		{
			var wSize = Window.SfmlWindow.Size;
			var bSize = _background.Texture.Size;

			var bRatio = bSize.X / (float)bSize.Y;
			var wRatio = wSize.X / (float)wSize.Y;

			float scale;
			if (wRatio > bRatio)
			{
				scale = wSize.X / (float)bSize.X;
			}
			else
			{
				scale = wSize.Y / (float)bSize.Y;
			}

			_background.Scale = new Vector2f(scale, scale);
		}

		private void OnMousePressed() => Controls.ForEach(x => x.MouseDown());

		private void OnMouseReleased() => Controls.ForEach(x => x.MouseUp());

		private void OnMouseMove(Vector2f position) => Controls.ForEach(x => x.MoveMouse(position));

		public void Dispose()
		{
			_controller.OnMousePressed -= OnMousePressed;
			_controller.OnMouseReleased -= OnMouseReleased;
			_controller.OnMouseMove -= OnMouseMove;

			_services.Dispose();
			_background.Dispose();

			foreach (var control in Controls)
			{
				if (control is IDisposable dispose)
				{
					dispose.Dispose();
				}
			}
		}

		public void Load()
		{
			_services.Setup();
		}
	}
}
