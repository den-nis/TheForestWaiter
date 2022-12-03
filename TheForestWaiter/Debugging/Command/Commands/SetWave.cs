using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Static;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setwave", "Reset and start the selected wave", "{wave number}")]
	internal class SetWave : ICommand
	{
		private readonly GameData _gameData;

		public SetWave(GameData gameData)
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

			var spawner = _gameData.Objects.Environment.FirstOrDefault(o => o is Spawner) as Spawner;
			spawner.StartWave(int.Parse(args[0]));
		}
	}
}
