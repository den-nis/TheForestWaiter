using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheForestWaiter.Environment;

namespace TheForestWaiter.Debugging
{
    static partial class GameDebug
    {
        public static Queue<Action<RenderWindow>> DrawQueue { get; set; } = new Queue<Action<RenderWindow>>();
        public static Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        public static GameData Game { get; set; }
        public static GameWindow Window { get; set; }

        private static BlockingCollection<string> PendingCommands { get; set; } = new BlockingCollection<string>();
        private const BindingFlags COMMAND_BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private static float Fps { get; set; }

        public static List<string> Logs { get; set; } = new List<string>();

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
            var msg = $"{DateTime.Now} {message}";
            if (GetVariable("output_log", true))
                Console.WriteLine(msg);
            
            Logs.Add(msg);
        }

        [Conditional("DEBUG")]
        public static void StartConsoleThread()
        {
            var task = Task.Run(() =>
            {
                while (true)
                {
                    PendingCommands.Add(Console.ReadLine());
                }
            });
        }

        [Conditional("DEBUG")]
        public static void Update(float time)
        {
            Fps = 1 / time;

            var lag = GetVariable("simulate_lag", 0);
            if (lag > 0)
                Thread.Sleep(lag);

            while(PendingCommands.Count > 0)
            {
                var command = PendingCommands.Take();

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
                Console.WriteLine($"Unknown command {command} try help");
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
            return GetAllCommandInfo().FirstOrDefault(c => c.Attribute.Name == name);
        }

        public static IEnumerable<CommandInfo> GetAllCommandInfo()
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
