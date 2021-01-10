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
        public static List<Vector2f> WorldCollisionChecks { get; set; } = new List<Vector2f>();
        public static List<(Vector2f position, Vector2f size)> HitBoxes { get; set; } = new List<(Vector2f, Vector2f)>();
        public static Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        public static GameData Game { get; set; } = null;
        private static ConcurrentQueue<string> CommandQueue { get; set; } = new ConcurrentQueue<string>();
        private static ManualResetEvent CommandHandled { get; set; } = new ManualResetEvent(false);
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
                    CommandQueue.Enqueue(Console.ReadLine());
                    while (!CommandQueue.IsEmpty)
                        CommandHandled.WaitOne();
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


            while(CommandQueue.TryDequeue(out string rawCommand))
            {
                try
                {
                    var parts = rawCommand.Split(' ');
                    ExecuteCommand(parts[0], parts[1..]);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                CommandHandled.Set();
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

        [Conditional("DEBUG")]
        public static void RegisterWorldCollisonCheck(Vector2f collision)
        {
            if (GetVariable("world_col_checks", false))
                WorldCollisionChecks.Add(collision);     
        }

        [Conditional("DEBUG")]
        public static void UpdateHitBox(Vector2f position, Vector2f size)
        {
            if (GetVariable("show_hitboxes", false))
                HitBoxes.Add((position, size));
        }

        [Conditional("DEBUG")]
        public static void Draw(RenderWindow window)
        {
            if (GetVariable("show_hitboxes", false))
            {
                RectangleShape r = new RectangleShape
                {
                    FillColor = Color.Transparent,
                    OutlineThickness = 1,
                    OutlineColor = Color.Green,
                };

                foreach (var (position, size) in HitBoxes)
                {
                    r.Position = position;
                    r.Size = size;
                    window.Draw(r);
                }

                HitBoxes.Clear();
            }

            if (GetVariable("world_col_checks", false))
            {
                RectangleShape r = new RectangleShape
                {
                    FillColor = new Color(255, 0, 0, 10),
                    Size = new Vector2f(World.TILE_SIZE, World.TILE_SIZE),
                };

                foreach (var c in WorldCollisionChecks)
                {
                    r.Position = c;
                    window.Draw(r);
                }
                WorldCollisionChecks.Clear();
            }
        }

        public static void DrawChunksUI(RenderWindow win)
        {
            var active = Game.Objects.Chunks.GetActiveChunks();

            RectangleShape chunkShape = new RectangleShape
            {
                Size = new Vector2f(Chunks.CHUNK_WIDTH / Camera.Scale, Game.World.Tiles.GetLength(1) * World.TILE_SIZE / Camera.Scale),
                FillColor = Color.Transparent,
                OutlineThickness = -1,
            };

            for (int i = 0; i < Game.Objects.Chunks.TotalChunks; i++)
            {
                chunkShape.OutlineColor = Color.Green;
                if (active.Contains(i))
                {
                    chunkShape.OutlineColor = Color.Red;
                }

                chunkShape.Position = Camera.ToCamera(new Vector2f(i * Chunks.CHUNK_WIDTH, 0));
                win.Draw(chunkShape);
            }
        }


        [Conditional("DEBUG")]
        public static void DrawUI(RenderWindow window)
        {
            if (GetVariable("draw_chunks", false))
                DrawChunksUI(window);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Fps: {Math.Round(Fps)}");
            sb.AppendLine($"Bullets: {Game?.Objects?.Bullets?.Count()}");
            sb.AppendLine($"Zoom: {Camera.Scale}");

            Text fpsText = new Text
            {
                Position = new Vector2f(0,0),
                DisplayedString = sb.ToString(),
                CharacterSize = 20,
                Font = GameContent.GetFont("Content.Fonts.arial.ttf"),
            };

            window.Draw(fpsText);
        }
    }
}
