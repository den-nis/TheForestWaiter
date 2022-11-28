using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("msg", "Send a message", "{message}")]
	internal class Msg : ICommand
	{
		private GameMessages _messages;
		private NetworkTraffic _traffic;

		public Msg(GameMessages messages, NetworkTraffic traffic)
        {
			_messages = messages;
			_traffic = traffic;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
            var name = _traffic.Username;

            if (_traffic.IsMultiplayer)
            {
			    _traffic.PostPublic($"{name}: {string.Join(' ', args)}");
            }
            else
            {
                _messages.PostLocal($"{name}: {string.Join(' ', args)}");
            }
		}
	}
}
