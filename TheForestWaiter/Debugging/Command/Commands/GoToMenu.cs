using LightInject;
using TheForestWaiter.UI.Menus;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("menu", "Go to the menu", "{name}")]
	internal class GoToMenu : ICommand
	{
		private readonly StateManager _stateManager;
		private readonly IServiceContainer _container;

		public GoToMenu(StateManager stateManager, IServiceContainer container)
		{
			_stateManager = stateManager;
			_container = container;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_stateManager.StartTransition(new StateTransition
			{
				TargetState = typeof(MainMenu),
				Length = 1,
			}, true);
		}
	}
}
