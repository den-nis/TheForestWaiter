using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Debugging
{
    public class CommandAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Usage { get; private set; }

        public CommandAttribute(string command)
        {
            Name = command;
        }

        public CommandAttribute(string command, string usage) : this(command)
        {
            Usage = usage;
        }

        public CommandAttribute(string command, string usage, string description) : this(command, usage)
        {
            Description = description;
        }
    }
}
