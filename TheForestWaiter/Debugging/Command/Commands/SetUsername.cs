using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setusername", "", "{new name}", CommandSupport.Host | CommandSupport.Client)]
	internal class SetUsername : ICommand
	{
		private readonly NetSettings _network;

		public SetUsername(NetSettings network)
		{
			_network = network;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_network.Username = args[0];
		}
	}
}
