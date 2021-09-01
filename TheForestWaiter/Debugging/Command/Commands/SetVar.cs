using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("set", "Set a variable", "set {name} {value}")]
    class SetVar : ICommand
    {
        public void Execute(object sender, string[] args)
        {
        }
    }
}
