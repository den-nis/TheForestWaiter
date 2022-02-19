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
        private readonly TimeProcessor _time;

        public Timescale(TimeProcessor time)
        {
            _time = time;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _time.TimeScale = float.Parse(args[0]);
        }
    }
}
