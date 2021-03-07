using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TheForestWaiter.Environment;
using TheForestWaiter.Objects;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging
{
    public static partial class Commands
    {
        [Command("setpos", "sets the player position", "setpos {x} {y}")]
        public static void SetPosition(string[] args)
        {  
            if (GameDebug.Game?.Objects?.Player != null)
            {
                GameDebug.Game.Objects.Player.Position = new Vector2f
                (
                    float.Parse(args[0]),
                    float.Parse(args[1])
                );
            }
            else
            {
                Console.WriteLine("There is no player");
            }
        }

        [Command("getpos", "gets the player position")]
        public static void GetPosition(string[] args)
        {
            if (GameDebug.Game?.Objects?.Player != null)
            {
                var pos = GameDebug.Game.Objects.Player.Position;
                Console.WriteLine($"x:{pos.X} y:{pos.Y}");
            }
            else
            {
                Console.WriteLine("There is no player");
            }
        }

        [Command("reboot", "Restart the game state")]
        public static void Reboot()
        {
            //TODO: make this less ulgy
            var sm = (StateManager)typeof(Program).GetField("_manager", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            sm.SetState(new GameState(GameDebug.Window));
        }

        [Command("spawn", "Spawns a creature", "spawn {name} ?{count} ?{x} ?{y}")]
        public static void Spawn(string[] args)
        {
            var creatures = GetCreatures();
            var spawn = creatures.First(x => x.Name == args[0]);

            for (int i = 0; i < (args.Length > 1 ? int.Parse(args[1]) : 1); i++)
            {
                Creature creature = (Creature)Activator.CreateInstance(spawn, new[] { GameDebug.Game });
                if (args.Length > 3)
                {
                    creature.Position = new Vector2f(float.Parse(args[2]), float.Parse(args[3]));
                }
                else
                {
                    creature.Position = GameDebug.Game.Objects.Player.Position + new Vector2f(0, -100);
                }

                GameDebug.Game.Objects.Enemies.Add(creature);
            }
        }

        [Command("creatures", "Shows a list of creatures")]
        public static void Creatures()
        {
            foreach(var i in GetCreatures())
			{
				Console.WriteLine($"\t- {i.Name}");
			}
        }

        private static Type[] GetCreatures()
		{
			var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().Where(t => typeof(Creature).IsAssignableFrom(t) && !t.IsAbstract).ToArray();
		}


        [Command("disarm", "No more shooting")]
        public static void Disarm()
        {
            GameDebug.Game.Objects.Player.Gun.Enabled = false;
        }

        [Command("arm", "Shoot")]
        public static void Arm()
        {
            GameDebug.Game.Objects.Player.Gun.Enabled = true;
        }

        [Command("mine", "minecraft?", "mine {radius}")]
        public static void Mine(string[] args)
        {
            var r = int.Parse(args[0]);
            var tiles = GameDebug.Game.World.GetTilesInArea(new Vector2f(r, r), GameDebug.Game.Objects.Player.Position);

            foreach(var i in tiles)
			{
                GameDebug.Game.World.Tiles[(int)i.Position.X / World.TILE_SIZE, (int)i.Position.Y / World.TILE_SIZE].Solid = false;
                GameDebug.Game.World.Tiles[(int)i.Position.X / World.TILE_SIZE, (int)i.Position.Y / World.TILE_SIZE].Air = true;
			}
        }
    }
}
