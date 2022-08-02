using SFML.System;
using System;
using TheForestWaiter.Game;
using TheForestWaiter.States;
using TheForestWaiter.UI.Abstract;

namespace TheForestWaiter.UI.Menus
{
	internal class MainMenu : UIState
	{ 
		public MainMenu()
		{
			var stateManager = IoC.GetInstance<StateManager>();

			int spacing = 100;
			int i = 0;

			var start = new Button("Textures/Menu/start.png", 3)
			{
				AbsolutePosition = new Vector2f(80, 80 + i++ * spacing),
			};
			var credits = new Button("Textures/Menu/credits.png", 3)
			{
				AbsolutePosition = new Vector2f(80, 80 + i++ * spacing),
			};
			var exit = new Button("Textures/Menu/exit.png", 3)
			{
				AbsolutePosition = new Vector2f(80, 80 + i++ * spacing),
			};

			start.OnClick += () => stateManager.StartTransition(new StateTransition
			{
				Length = 4,
				TargetState = typeof(GameState),
			});

			exit.OnClick += () =>
			{
				if (!stateManager.IsTransitioning)
				{
					Environment.Exit(0);
				}
			};

			credits.OnClick += () => stateManager.StartTransition(new StateTransition
			{
				TargetState = typeof(CreditsMenu),
			});

			Controls.AddRange(new Control[] {
				start,
				credits,
				exit,
			});
		}
	}
}
