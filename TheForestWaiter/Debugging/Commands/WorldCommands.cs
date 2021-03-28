using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TheForestWaiter.Environment;
using TheForestWaiter.Objects;
using TheForestWaiter.Objects.Weapons;
using TheForestWaiter.Objects.Weapons.Guns;
using TheForestWaiter.States;

namespace TheForestWaiter.Debugging
{
    public static partial class Commands
    {
        [Command("setpos", "sets the player position", "setpos {x} {y}")]
        public static void SetPosition(string[] args)
        {  
            GameDebug.Game.Objects.Player.Position = new Vector2f
            (
                float.Parse(args[0]),
                float.Parse(args[1])
            );
        }

        [Command("getpos", "gets the player position")]
        public static void GetPosition()
        {
            var pos = GameDebug.Game.Objects.Player.Position;
            Console.WriteLine($"x:{pos.X} y:{pos.Y}");
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
            return GetAssignableTypes(typeof(Creature));
		}

        private static Type[] GetAssignableTypes(Type type)
		{
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && !t.IsAbstract).ToArray();
        }

        [Command("disarm", "No more shooting")]
        public static void Disarm()
        {
            GameDebug.Game.Objects.Player.RemoveGun();
        }

        [Command("arm", "Equip a gun", "arm ?{gun}")]
        public static void Arm(string[] args)
        {
            if (args.Length > 0)
			{
                var guns = GetAssignableTypes(typeof(GunBase));
                var gun = guns.FirstOrDefault(g => g.Name == args[0]);

                if (gun != null)
				{
                    GameDebug.Game.Objects.Player.SetGun((GunBase)Activator.CreateInstance(gun, new object[] { GameDebug.Game } ));
                    return;
                }
                else
				{
                    Console.WriteLine($"Could not find gun \"{args[0]}\"");
                    return;
                }

            }

            GameDebug.Game.Objects.Player.SetGun(new Handgun(GameDebug.Game));
        }
    }
}
