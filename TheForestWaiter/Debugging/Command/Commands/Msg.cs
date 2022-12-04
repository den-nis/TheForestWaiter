using TheForestWaiter.Game;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("msg", "Send a message", "{message}", CommandSupport.All)]
	internal class Msg : ICommand
	{
		private GameMessages _messages;
		private NetContext _network;

		public Msg(GameMessages messages, NetContext network)
        {
			_messages = messages;
			_network = network;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
            var name = _network.Settings.Username;
			_messages.Post($"{name}: {string.Join(' ', args)}");
		}
	}
}
