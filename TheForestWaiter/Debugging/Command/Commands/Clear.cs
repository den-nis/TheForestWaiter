using System;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("clear", "Clears the console", "", CommandSupport.All)]
	internal class Clear : ICommand
	{
		public void Execute(CommandHandler handler, string[] args)
		{
			Console.Clear();
		}
	}
}
