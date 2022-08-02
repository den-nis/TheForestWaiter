using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.UI
{
	internal abstract class Control
	{
		public Vector2f Position { get; set; }
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
			if (Collisions.BoxPoint(Position, Size, _mousePosition))
				OnPressed();
		}

		public void MouseUp()
		{
			if (Collisions.BoxPoint(Position, Size, _mousePosition))
				OnReleased();
		}

		public void MoveMouse(Vector2f position)
		{
			_mousePosition = position;
			if (Collisions.BoxPoint(Position, Size, position))
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
