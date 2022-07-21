using System;

namespace TheForestWaiter.Debugging.Command
{
	[AttributeUsage(AttributeTargets.Class)]
	class CommandAttribute : Attribute
	{
		public string Name { get; private set; }
		public string Description { get; private set; }
		public string Usage { get; private set; }

		public CommandAttribute(string command)
		{
			Name = command;
		}

		public CommandAttribute(string command, string description) : this(command)
		{
			Description = description;
		}

		public CommandAttribute(string command, string description, string usage) : this(command, description)
		{
			Usage = usage;
		}
	}
}
