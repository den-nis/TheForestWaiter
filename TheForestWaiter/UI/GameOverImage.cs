using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.UI.Abstract;

namespace TheForestWaiter.UI
{
	internal class GameOverImage : Control
	{
		private readonly WindowHandle _window;
		private readonly Sprite _sprite;

		private const float SCALE = 3;

		public GameOverImage()
		{
			var content = IoC.GetInstance<ContentSource>();
			_window = IoC.GetInstance<WindowHandle>();

			_sprite = content.Textures.CreateSprite("Textures/Menu/death.png");
			_sprite.Scale = new Vector2f(SCALE, SCALE);
			_sprite.Origin = _sprite.Texture.Size.ToVector2f() / 2;
		}

		public override void Update(float time)
		{
			var center = _window.SfmlWindow.Size.ToVector2f() / 2;
			_sprite.Position = center;
		}

		public override void Draw(RenderWindow window)
		{
			window.Clear();
			window.Draw(_sprite);
		}
	}
}
