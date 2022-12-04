using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Static;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("nospawn", "Delete all enemies and disable future spawning", "", CommandSupport.All)]
	internal class NoSpawn : ICommand
	{
		private readonly GameData _gameData;
		private readonly NetContext _net;

		public NoSpawn(GameData gameData, NetContext net)
		{
			_gameData = gameData;
			_net = net;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			if (_net.Settings.IsClient)
			{
				handler.InjectCommand($"exec nospawn");
			}
			else
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
}
