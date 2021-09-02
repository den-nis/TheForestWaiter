using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("usage", "Shows how to use a command", "{command}")]
    class Usage : ICommand
    {
        public void Execute(CommandHandler handler, string[] args)
        {
            var commands = handler.CommandInfo[args[0]];
            Console.WriteLine(commands.Attribute.Usage ?? $"command \"{args[0]}\" has no parameters");
        }
    }
}
