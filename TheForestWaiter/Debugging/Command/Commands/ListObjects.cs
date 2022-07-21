using System;
using TheForestWaiter.Debugging.DebugConsole;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("list", "Show names of objects")]
	internal class ListObjects : ICommand
	{
		public void Execute(CommandHandler handler, string[] args)
		{
			var tb = new TableBuilder();
			foreach (var obj in Types.GameObjects.Values)
			{
				tb.WriteRow("Object", obj.Name);
			}

			foreach (var obj in Types.Weapons.Values)
			{
				tb.WriteRow("Weapons", obj.Name);
			}

			Console.WriteLine(tb);
		}
	}
}
