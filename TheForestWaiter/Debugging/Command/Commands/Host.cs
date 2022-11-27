using LightInject;
using System.Net;
using TheForestWaiter.Game;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("host", "Change the current state to gamestate", "{port}")]
	internal class Host : ICommand
	{
		private readonly StateManager _stateManager;
		private readonly NetworkSettings _network;
		private readonly IServiceContainer _container;

		public Host(StateManager stateManager, NetworkSettings network, IServiceContainer container)
		{
			_stateManager = stateManager;
			_network = network;
			_container = container;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_network.Setup(true, IPAddress.Any, short.Parse(args[0]));

			var state = _container.GetInstance<GameState>();
			_stateManager.ForceClearQueue();
			_stateManager.SetState(state);
		}
	}
}
