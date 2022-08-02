using LightInject;
using TheForestWaiter.Game;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("restart", "Change the current state to gamestate", "{name} ?{amount} ?{x} ?{y}")]
	internal class Restart : ICommand
	{
		private readonly StateManager _stateManager;
		private readonly IServiceContainer _container;

		public Restart(StateManager stateManager, IServiceContainer container)
		{
			_stateManager = stateManager;
			_container = container;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_stateManager.ForceClearQueue();
			_stateManager.SetState(_container.GetInstance<GameState>());
		}
	}
}
