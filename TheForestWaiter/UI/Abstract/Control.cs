using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.UI.Abstract
{
	internal abstract class Control
	{
		private readonly WindowHandle _window;

		private bool _maintainY;
		private bool _maintainX;

		public Control()
		{
			_window = IoC.GetInstance<WindowHandle>();
		}

		public void SetYRelativeBottom() => _maintainY = true;

		public void SetXRelativeToRight() => _maintainX = true;

		protected Vector2f ActualPosition 
		{
			get
			{
				var x = _maintainX ? _window.SfmlWindow.Size.X - AbsolutePosition.X : AbsolutePosition.X;
				var y = _maintainY ? _window.SfmlWindow.Size.Y - AbsolutePosition.Y : AbsolutePosition.Y;
				return new Vector2f(x, y);
			}
		}

		public Vector2f AbsolutePosition { get; set; }
		public Vector2f Size { get; set; }

		private bool _mouseIsInside = false;
		private Vector2f _mousePosition;

		public abstract void Update(float time);
		public abstract void Draw(RenderWindow window);

		protected virtual void OnReleased() { }
		protected virtual void OnPressed() { }
		protected virtual void OnMouseMoveEnter(Vector2f position) { }
		protected virtual void OnMouseMoveExit(Vector2f position) { }

		public void MouseDown()
		{
			if (Collisions.BoxPoint(ActualPosition, Size, _mousePosition))
				OnPressed();
		}

		public void MouseUp()
		{
			if (Collisions.BoxPoint(ActualPosition, Size, _mousePosition))
				OnReleased();
		}

		public void MoveMouse(Vector2f position)
		{
			_mousePosition = position;
			if (Collisions.BoxPoint(ActualPosition, Size, position))
			{
				if (!_mouseIsInside)
					OnMouseMoveEnter(position);

				_mouseIsInside = true;
			}
			else
			{
				if (_mouseIsInside)
					OnMouseMoveExit(position);

				_mouseIsInside = false;
			}
		}
	}
}
