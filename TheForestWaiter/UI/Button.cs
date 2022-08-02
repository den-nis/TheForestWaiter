using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.UI
{
	internal class Button : Control, IDisposable
	{
		public event Action OnClick;

		private readonly Sprite _sprite;

		private bool hovering = false;
		private bool pressed = false;

		public Button(string texture, float scale)
		{
			var content = IoC.GetInstance<ContentSource>();

			_sprite = content.Textures.CreateSprite(texture);
			_sprite.Scale = new Vector2f(scale, scale);
			Size = _sprite.Texture.Size.ToVector2f() * scale;
		}

		public override void Update(float time)
		{
			byte alpha;

			if (hovering)
			{
				alpha = pressed ? (byte)80 : (byte)200;
			}
			else
			{
				alpha = 255;
			}

			var c = _sprite.Color;
			_sprite.Color = new Color(c.R, c.G, c.B, alpha);
			_sprite.Position = Position;
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_sprite);
		}

		protected override void OnReleased()
		{
			pressed = false;
			OnClick?.Invoke();
		}
		
		protected override void OnPressed() => pressed = true;
		
		protected override void OnMouseMoveEnter(Vector2f position) => hovering = true;
		
		protected override void OnMouseMoveExit(Vector2f position)
		{
			hovering = false;
			pressed = false;
		}

		public void Dispose()
		{
			_sprite.Dispose();
		}
	}
}
