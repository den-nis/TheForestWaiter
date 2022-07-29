using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Static;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setwave", "Start the target wave", "setwave 2")]
	internal class SetWave : ICommand
	{
		private readonly GameData _gameData;

		public SetWave(GameData gameData)
		{
			_gameData = gameData;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			foreach(var creature in _gameData.Objects.Creatures.Where(c => !c.Friendly))
			{
				creature.Delete();
				Console.WriteLine($"Deleted {creature.GetType().Name} ({creature.GameObjectId})");
			}

			var spawner = _gameData.Objects.Environment.FirstOrDefault(o => o is Spawner) as Spawner;
			spawner.StartWave(int.Parse(args[0]));
		}
	}
}
