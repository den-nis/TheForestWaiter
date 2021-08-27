using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TheForestWaiter.Debugging.Command
{
    class CommandInfo
    {
        public ICommand Command { get; set; }
        public CommandAttribute Attribute { get; set; }
    }
}
