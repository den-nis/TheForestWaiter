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
        [Command("setpos", "setpos {x} {y}", "sets the player position")]
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
    }
}
