using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command
{
	class CommandHandler : IDisposable
	{
        private Task _consoleThread;

        public IReadOnlyDictionary<string, CommandInfo> CommandInfo => _commands;
        private readonly BlockingCollection<string> _pendingCommands = new();
        private readonly Dictionary<string, CommandInfo> _commands = new();
        private readonly Dictionary<string, ICommand> _commandCache = new();
		private GameData _game;

        public void ProvideGameData(GameData game)
        {
            _game = game;
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

        private void ExecuteCommand(string command, string[] args)
        {
            if (!_commands.ContainsKey(command))
            {
                Console.WriteLine($"Unknown command {command}, use \"help\" to get a list of commands");
                return;
            }

            if (_commandCache.TryGetValue(command, out var instance))
            {
                instance.Execute(this, args);
            }
            else
            {
                instance = CreateCommand(command);
                _commandCache.Add(command, instance);

                instance.Execute(this, args);
            }
        }

        private ICommand CreateCommand(string name)
        {
            Type command = _commands[name].Command;
            ICommand instance;

            if (command.GetConstructor(Array.Empty<Type>()) != null)
            {
                instance = (ICommand)Activator.CreateInstance(command);
            }
            else
            {
                if (_game == null)
                    throw new InvalidOperationException("Cannot create this command without gameData");

                instance = (ICommand)Activator.CreateInstance(command, _game);
            }

            return instance;
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
