using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging.DebugConsole;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("objects", "Show names of objects that can be spawned")]
    class ListObjects : ICommand
    {
        public void Execute(CommandHandler handler, string[] args)
        {
            var tb = new TableBuilder();
            foreach(var obj in Types.GameObjects)
            {
                tb.WriteRow(obj.Name);
            }

            Console.WriteLine(tb);
        }
    }
}
