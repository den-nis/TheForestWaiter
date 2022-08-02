using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("kill", "Kill the player")]
	internal class Kill : ICommand
	{
		private readonly GameData _gameData;

		public Kill(GameData gameData)
		{
			_gameData = gameData;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			var player = _gameData.Objects.Player;
			player.Damage(null, 9001, 0);
		}
	}
}
