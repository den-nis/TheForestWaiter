using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("msg", "Send a message", "{message}")]
	internal class Msg : ICommand
	{
		private GameMessages _messages;
		private NetworkSettings _network;

		public Msg(GameMessages messages, NetworkSettings network)
        {
			_messages = messages;
			_network = network;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
            var name = _network.Username;

            if (_network.IsMultiplayer)
            {
			    _messages.PostPublic($"{name}: {string.Join(' ', args)}");
            }
            else
            {
                _messages.PostLocal($"{name}: {string.Join(' ', args)}");
            }
		}
	}
}
