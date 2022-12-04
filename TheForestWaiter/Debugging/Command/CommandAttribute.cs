using System;
using System.Linq;

namespace TheForestWaiter.Debugging.Command
{
	[Flags]
	internal enum CommandSupport
	{
		SinglePlayer = 2,
		Client = 4,
		Host = 8,
		All = SinglePlayer | Client | Host,
	}

	[AttributeUsage(AttributeTargets.Class)]
	internal class CommandAttribute : Attribute
	{
		public CommandSupport Support { get; private set; } = CommandSupport.SinglePlayer;
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

		public CommandAttribute(string command, string description, string usage, CommandSupport support) : this(command, description, usage)
		{
			Support = support;
		}
	}
}
