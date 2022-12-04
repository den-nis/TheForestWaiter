using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("exec", "Run a command on the host or on all clients if you are host", "{command}", CommandSupport.Client | CommandSupport.Host)]
	internal class ExecCommand : ICommand
	{
		private readonly NetContext _context;

		public ExecCommand(NetContext context)
		{
			_context = context;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_context.Traffic.Send(new Multiplayer.Messages.RemoteCommand
			{
				Cmd = string.Join(' ', args),
			});
		}
	}
}
