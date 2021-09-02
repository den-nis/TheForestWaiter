using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("timescale", "Speed up or slow time", "{number}")]
    class Timescale : ICommand
    {
        private readonly Entry gameEntry;

        public Timescale(Entry gameEntry)
        {
            this.gameEntry = gameEntry;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            gameEntry.TimeScale = float.Parse(args[0]);
        }
    }
}
