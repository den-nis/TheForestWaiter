using SFML.System;
using TheForestWaiter.States;
using TheForestWaiter.UI.Abstract;

namespace TheForestWaiter.UI.Menus
{
	internal class DeathMenu : UIState
	{ 
		public DeathMenu()
		{
			var state = IoC.GetInstance<StateManager>();

			var score = new ScoreLabel()
			{
				AbsolutePosition = new Vector2f(80, 80)
			};

			var menu = new Button("Textures/Menu/return.png", 3)
			{
				AbsolutePosition = new Vector2f(30, 80),
			};
			menu.SetYRelativeBottom();

			menu.OnClick += () => state.StartTransition(new StateTransition { TargetState = typeof(MainMenu) });

			Controls.AddRange(new Control[] {
				new GameOverImage(),
				score,
				menu,
			});
		}
	}
}
