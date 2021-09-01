using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("usage", "Shows how to use a command", "usage {command}")]
    class Usage : ICommand
    {
        public void Execute(object sender, string[] args)
        {
            if (sender is CommandHandler handler)
            {
                var commands = handler.CommandInfo[args[0]];
                Console.WriteLine(commands.Attribute.Usage ?? "command has no parameters");
            }
        }
    }
}
