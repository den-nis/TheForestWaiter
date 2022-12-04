using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Static;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setwave", "Reset and start the selected wave", "{wave number}", CommandSupport.All)]
	internal class SetWave : ICommand
	{
		private readonly GameData _gameData;
		private readonly NetContext _net;

		public SetWave(GameData gameData, NetContext net)
		{
			_gameData = gameData;
			_net = net;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			if (_net.Settings.IsClient)
			{
				handler.InjectCommand($"exec setwave {int.Parse(args[0])}");
			}
			else
			{
				foreach (var creature in _gameData.Objects.Creatures.Where(c => !c.Friendly))
				{
					Console.WriteLine($"Deleted {creature.GetType().Name}");
					creature.Delete();
				}

				var spawner = _gameData.Objects.Environment.FirstOrDefault(o => o is Spawner) as Spawner;
				spawner.Restart();
				spawner.StartWave(int.Parse(args[0]));
			}
		}
	}
}
