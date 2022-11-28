﻿using LightInject;
using System.Net;
using TheForestWaiter.Game;
using TheForestWaiter.Multiplayer.Messages;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("join", "", "{ip} {port}")]
	internal class Join : ICommand
	{
		private readonly StateManager _stateManager;
		private readonly NetworkSettings _network;
		private readonly UserSettings _settings;
		private readonly IServiceContainer _container;

		public Join(StateManager stateManager,
			  NetworkSettings network,
			  UserSettings settings,
			  IServiceContainer container)
		{
			_stateManager = stateManager;
			_network = network;
			_settings = settings;
			_container = container;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_network.Setup(false, IPAddress.Parse(args[0]), short.Parse(args[1]));

			var state = _container.GetInstance<GameState>();
			_stateManager.ForceClearQueue();
			_stateManager.SetState(state);
		}
	}
}