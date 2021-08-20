using SFML.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging
{
    static partial class GameDebug
    {
        public static Queue<Action<RenderWindow>> DrawQueue { get; set; } = new Queue<Action<RenderWindow>>();
        public static Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        public static List<string> Logs { get; set; } = new List<string>();

        private static GameData _game;
        private static GameWindow _window;

        public static GameData Game 
        {
            get
			{
#if !DEBUG
                throw new InvalidOperationException("Cannot use debug resources in release");
#endif
                return _game;
            }
            set => _game = value;
        }
        public static GameWindow Window
        {
            get
            {
#if !DEBUG
                throw new InvalidOperationException("Cannot use debug resources in release");
#endif
                return _window;
            }
            set => _window = value;
        }

        private static readonly BlockingCollection<string> _pendingCommands = new();
        private const BindingFlags COMMAND_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private static float _fps;

        public static T GetVariable<T>(string name, T defaultValue)
        {
#if DEBUG
            if (Variables.TryGetValue(name, out object value))
                return (T)value;

            Variables.Add(name, defaultValue);
#endif
            return defaultValue;
        }

        [Conditional("DEBUG")]
        public static void ProvideGameData(GameData game)
        {
            Game = game;
        }

        [Conditional("DEBUG")]
        public static void ProvideWindow(GameWindow window)
		{
            Window = window;
        }

        [Conditional("DEBUG")]
        public static void Log(string message)
        {
            var msg = $"{DateTime.Now.ToShortTimeString()} {message}";
            Logs.Add(msg);
        }

        [Conditional("DEBUG")]
        public static void StartConsoleThread()
        {
            var task = Task.Run(() =>
            {
                while (true)
                {
                    _pendingCommands.Add(Console.ReadLine());
                }
            });
        }

        [Conditional("DEBUG")]
        public static void Update(float time)
        {
            _fps = 1 / time;

            var lag = GetVariable("simulate_lag", 0);
            if (lag > 0)
                Thread.Sleep(lag);

            while(_pendingCommands.Count > 0)
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

        private static void ExecuteCommand(string command, string[] args)
        {
            var com = GetCommandInfo(command);

            if (com == null)
            {
                Console.WriteLine($"Unknown command {command}, use \"help\" to get a list of commands");
                return;
            }

            if (com.Method.GetParameters().Length > 0)
            {
                com.Method.Invoke(null, new[] { args });
            }
            else
            {
                com.Method.Invoke(null, null);
            }   
        }

        public static CommandInfo GetCommandInfo(string name)
        {
#if !DEBUG
            throw new InvalidOperationException("Cannot use debug resources in release");
#endif

            return GetAllCommandInfos().FirstOrDefault(c => c.Attribute.Name == name);
        }

        public static IEnumerable<CommandInfo> GetAllCommandInfos()
        {
            return typeof(Commands).GetMethods(COMMAND_BINDINGS).Select(m => 
                new CommandInfo
                {
                    Method = m,
                    Attribute = m.GetCustomAttribute<CommandAttribute>(),
                }
            ).Where(c => c.Attribute != null);
        }

        [Conditional("DEBUG")]
        public static void LogDeletions<T>(IEnumerable<object> objects)
        {
            if (objects.Any())
                Log($"Clearing {typeof(T).Name} {string.Join(',', objects.Select(x => x.GetType().Name).Distinct())}");
        }
    }
}
