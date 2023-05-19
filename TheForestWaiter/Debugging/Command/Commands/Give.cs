using LightInject;
using System;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("give", "equip a weapon", "{weapon name}")]
	internal class Give : ICommand
	{
		private readonly GameData _game;
		private readonly IServiceContainer _container;

		public Give(GameData game, IServiceContainer container)
		{
			_game = game;
			_container = container;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			var weapon = (Weapon)_container.GetInstance(Types.Weapons.Values.FirstOrDefault(t => t.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase)));
			_game.Objects.Player.WeaponWheel.Add(weapon);
		}
	}
}
