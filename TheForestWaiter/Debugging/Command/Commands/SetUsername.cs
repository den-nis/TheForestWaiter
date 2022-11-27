namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setusername", "", "{new name}")]
	internal class SetUsername : ICommand
	{
		private readonly NetworkSettings _network;

		public SetUsername(NetworkSettings network)
		{
			_network = network;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_network.Username = args[0];
		}
	}
}
