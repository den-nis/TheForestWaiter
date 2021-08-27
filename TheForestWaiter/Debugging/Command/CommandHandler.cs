using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command
{
	class CommandHandler : IDisposable
	{
        private Task _consoleThread;

        private readonly BlockingCollection<string> _pendingCommands = new();
        private readonly Dictionary<string, CommandInfo> _commands = new();
		private readonly GameData _game;

		public CommandHandler(GameData game)
		{
			_game = game;
		}

        public void Setup()
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

            _commands[command].Command.Execute(args);
        }

        private void IndexCommands()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var commandTypes = types.Where(t => t.IsSubclassOf(typeof(ICommand)));

            foreach (var command in commandTypes)
            {
                ICommand instance = (command.GetConstructor(Array.Empty<Type>()) != null) ?
                    (ICommand)Activator.CreateInstance(command) :
                    (ICommand)Activator.CreateInstance(command, _game);

                var attribute = instance.GetType().GetCustomAttribute<CommandAttribute>();

                _commands.Add(attribute.Name, new CommandInfo
                {
                    Command = instance,
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
