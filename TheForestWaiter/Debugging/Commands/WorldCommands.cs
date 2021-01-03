using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Objects;
using TheForestWaiter.Resources;

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
    }
}
