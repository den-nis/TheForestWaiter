using LightInject;
using System;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("hitbox", "Toggle hitboxes", "", CommandSupport.All)]
	internal class HitBoxes : ICommand
	{
		private readonly GameData _game;

		public HitBoxes(IServiceContainer provider, GameData game)
		{
			_game = game;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			_game.Objects.EnableDrawHitBoxes = !_game.Objects.EnableDrawHitBoxes;
			Console.WriteLine(_game.Objects.EnableDrawHitBoxes ? "Enabled hitboxes" : "Disabled hitboxes");
		}
	}
}
