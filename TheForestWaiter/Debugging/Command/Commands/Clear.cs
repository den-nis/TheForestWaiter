using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("clear", "Clears the console")]
    internal class Clear : ICommand
    {
        public void Execute(CommandHandler handler, string[] args)
        {
            Console.Clear();
        }
    }
}
