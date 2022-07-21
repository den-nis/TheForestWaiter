namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("timescale", "Speed up or slow time", "{number}")]
    internal class Timescale : ICommand
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
