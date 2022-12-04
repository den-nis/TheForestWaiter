using LightInject;
using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("spawn", "Spawn a creature", "{name} ?{amount} ?{x} ?{y}", CommandSupport.All)]
	internal class Spawn : ICommand
	{
		private readonly IServiceContainer _container;
		private readonly GameData _game;
		private readonly NetContext _net;

		public Spawn(IServiceContainer provider, GameData game, NetContext net)
		{
			_container = provider;
			_game = game;
			_net = net;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			int amount = args.Length > 1 ? int.Parse(args[1]) : 1;

			var pos = _game.Objects.Player.Position;

			if (args.Length > 2)
				pos.X = float.Parse(args[2]);

			if (args.Length > 3)
				pos.Y = float.Parse(args[3]);

			if (_net.Settings.IsClient)
			{
				var command = $"exec spawn {args[0]} {amount} {pos.X} {pos.Y}";
				handler.InjectCommand(command);
			}
			else
			{
				var type = Types.GameObjects.Values.FirstOrDefault(t => t.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));

				for (int i = 0; i < amount; i++)
				{
					var instance = (GameObject)_container.GetInstance(type);
					instance.Center = pos;
					_game.Objects.AddGameObject(instance);
				}
			}
		}
	}
}
