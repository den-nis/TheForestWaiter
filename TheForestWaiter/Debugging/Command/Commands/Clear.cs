using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("clear", "Clears the console")]
    class Clear : ICommand
    {
        public void Execute(object sender, string[] args)
        {
            Console.Clear();
        }
    }
}
