using System;

namespace TheForestWaiter.Debugging.Command
{
	class CommandInfo
    {
        public Type Command { get; set; }
        public CommandAttribute Attribute { get; set; }
    }
}
