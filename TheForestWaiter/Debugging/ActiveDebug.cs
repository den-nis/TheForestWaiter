﻿using LightInject;
using SFML.Graphics;
using TheForestWaiter.Debugging.Command;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Debugging
{
	internal class ActiveDebug : IDebug
	{
		private readonly IServiceContainer _serviceContainer;
		private CommandHandler _commandHandler;

		public ActiveDebug(IServiceContainer serviceContainer)
		{
			_serviceContainer = serviceContainer;
		}

		public void Draw(RenderWindow window)
		{
		}

		public void Setup()
		{
			_commandHandler = new CommandHandler(_serviceContainer);
			_commandHandler.IndexAndStartConsoleThread();
		}

		public void Update(float time)
		{
			_commandHandler.Update();
		}

		public void Dispose()
		{

		}
	}
}
