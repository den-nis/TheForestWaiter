using LightInject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command
{
	class CommandHandler : IDisposable
	{
		private Task _consoleThread;

		public IReadOnlyDictionary<string, CommandInfo> CommandInfo => _commands;
		private readonly BlockingCollection<string> _pendingCommands = new();
		private readonly Dictionary<string, CommandInfo> _commands = new();
		private readonly IServiceContainer _container;

		public CommandHandler(IServiceContainer container)
		{
			_container = container;
		}

		public void IndexAndStartConsoleThread()
		{
			IndexCommands();
			_consoleThread = StartConsoleThread();
		}

		public void Update()
		{
			while (_pendingCommands.Count > 0)
			{
				var command = _pendingCommands.Take();

				if (string.IsNullOrEmpty(command))
				{
					continue;
				}

				try
				{
					var parts = command.Split(' ');
					ExecuteCommand(parts[0], parts[1..]);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		private Task StartConsoleThread()
		{
			return Task.Factory.StartNew(() =>
			{
				while (true)
				{
					_pendingCommands.Add(Console.ReadLine());
				}
			}, TaskCreationOptions.LongRunning);
		}

		public void InjectCommand(string[] command) => InjectCommand(string.Join(' ', command));
		
		public void InjectCommand(string command)
		{
			_pendingCommands.Add(command);
		}

		private void ExecuteCommand(string command, string[] args)
		{
			if (!_commands.ContainsKey(command))
			{
				Console.WriteLine($"Unknown command {command}, use \"help\" to get a list of commands");
				return;
			}

			var instance = CreateCommand(command);
			instance.Execute(this, args);
		}

		private ICommand CreateCommand(string name)
		{
			Type command = _commands[name].Command;
			return (ICommand)_container.GetInstance(command);
		}

		private void IndexCommands()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var types = assembly.GetTypes();
			var commandTypes = types.Where(t => t.IsAssignableTo(typeof(ICommand)) && t != typeof(ICommand));

			foreach (var command in commandTypes)
			{
				var attribute = command.GetCustomAttribute<CommandAttribute>();

				_commands.Add(attribute.Name, new CommandInfo
				{
					Command = command,
					Attribute = attribute
				});
			}
		}

		public void Dispose()
		{
			_consoleThread.Dispose();
		}
	}
}
