using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Environment.Spawning;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setwave", "Start the target wave", "setwave 2")]
	internal class SetWave : ICommand
	{
		private readonly GameData _gameData;
		private readonly SpawnScheduler _schedule;

		public SetWave(GameData gameData)
		{
			_gameData = gameData;
			_schedule = IoC.GetInstance<SpawnScheduler>();
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			foreach (var creature in _gameData.Objects.Creatures.Where(c => !c.Friendly))
			{
				creature.Delete();
				Console.WriteLine($"Deleted {creature.GetType().Name} ({creature.GameObjectId})");
			}

			_schedule.FastForwardToWave(int.Parse(args[0]));
		}
	}
}
