using System;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("fixdelta", "Fix delta time to a specific amount", "{number}")]
    internal class FixDeltaTime : ICommand
    {
        private readonly TimeProcessor _time;

        public FixDeltaTime(TimeProcessor time)
        {
            _time = time;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _time.LockDelta = float.Parse(args[0]);
        }
    }
}
