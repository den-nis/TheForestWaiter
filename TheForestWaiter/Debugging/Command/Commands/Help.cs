using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging.DebugConsole;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("help", "Shows commands")]
    class Help : ICommand
    {
        public void Execute(CommandHandler handler, string[] args)
        {
            var tb = new TableBuilder(true);

            tb.WriteRow("Name", "Description", "Parameters");
            foreach (var m in handler.CommandInfo.Values)
            {
                tb.WriteRow(m.Attribute.Name, m.Attribute.Description, m.Attribute.Usage);
            }

            Console.WriteLine(tb);
            Console.WriteLine("use \"usage {command}\" to get info about parameters");
        }
    }
}
