using System;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("fixdelta", "Fix delta time to a specific amount", "{number}")]
    class FixDeltaTime : ICommand
    {
        private readonly Entry _main;

        public FixDeltaTime(Entry main)
        {
            _main = main;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _main.LockDelta = float.Parse(args[0]);
        }
    }
}
