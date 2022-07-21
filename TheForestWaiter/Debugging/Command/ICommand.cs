namespace TheForestWaiter.Debugging.Command
{
	interface ICommand
	{
		void Execute(CommandHandler handler, string[] args);
	}
}
