using System;
using TheForestWaiter.Debugging.DebugConsole;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("timeinfo", "", "")]
    internal class TimeInfo : ICommand
    {
        private readonly TimeProcessor _time;

        public TimeInfo(TimeProcessor time)
        {
            _time = time;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            var tb = new TableBuilder(false);

            tb.WriteRow("Time scale", _time.TimeScale.ToString());
            tb.WriteRow("Time difference", _time.TimeDifference.ToString());
            tb.WriteRow("Average delta", _time.AverageDelta.ToString());
            tb.WriteRow("Lag limit", _time.LagLimit.ToString());
            tb.WriteRow("Framterate", _time.Framerate.ToString());
            tb.WriteRow("Average time difference", _time.AverageTimeDifference.ToString());

            Console.WriteLine(tb);
        }
    }
}
