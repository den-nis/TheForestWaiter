using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Static;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("nospawn", "Delete all enemies and disable future spawning")]
	internal class NoSpawn : ICommand
	{
		private readonly GameData _gameData;

		public NoSpawn(GameData gameData)
		{
			_gameData = gameData;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			foreach (var creature in _gameData.Objects.Creatures.Where(c => !c.Friendly))
			{
				Console.WriteLine($"Deleted {creature.GetType().Name}");
				creature.Delete();
			}

			_gameData.Objects.Environment.FirstOrDefault(o => o is Spawner)?.Delete();
		}
	}
}
