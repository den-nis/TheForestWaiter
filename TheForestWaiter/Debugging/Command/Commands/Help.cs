using System;
using TheForestWaiter.Debugging.DebugConsole;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("help", "Shows commands", "", CommandSupport.All)]
	internal class Help : ICommand
	{
		public void Execute(CommandHandler handler, string[] args)
		{
			var tb = new TableBuilder(true);

			tb.WriteRow("Name", "Description", "Parameters", "Support");
			foreach (var m in handler.CommandInfo.Values)
			{
				tb.WriteRow(m.Attribute.Name, m.Attribute.Description, m.Attribute.Usage, m.Attribute.Support);
			}

			Console.WriteLine(tb);
		}
	}
}
