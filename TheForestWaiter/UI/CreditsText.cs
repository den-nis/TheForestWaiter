using SFML.Graphics;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.States;
using TheForestWaiter.UI.Menus;

namespace TheForestWaiter.UI
{
	internal class CreditsText : Control
	{
		private readonly WindowHandle _window;
		private readonly StateManager _stateManager;
		private readonly Sprite _sprite;

		private const float DURATION = 8;
		private float _timer = 0;
		private bool _finished = false;

		public CreditsText()
		{
			var content = IoC.GetInstance<ContentSource>();
			_window = IoC.GetInstance<WindowHandle>();
			_stateManager = IoC.GetInstance<StateManager>();

			_sprite = content.Textures.CreateSprite("Textures/Menu/credits_screen.png");
		}

		public override void Update(float time)
		{
			var center = _window.SfmlWindow.Size.ToVector2f() / 2;
			_sprite.Position = center - _sprite.Texture.Size.ToVector2f() / 2;

			if (_timer > DURATION && !_finished)
			{
				_stateManager.StartTransition(new StateTransition
				{
					Length = 5,
					TargetState = typeof(MainMenu),
				}, true);

				_finished = true;
			}
			else
			{
				_timer += time;
			}
		}

		public override void Draw(RenderWindow window)
		{
			window.Clear();
			window.Draw(_sprite);
		}
	}
}
