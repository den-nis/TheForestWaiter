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
		private readonly ItemRepository _repo;
		private readonly IServiceContainer _container;

		public Give(GameData game, ItemRepository repo, IServiceContainer container)
		{
			_game = game;
			_repo = repo;
			_container = container;
		}

		public void Execute(CommandHandler handler, string[] args)
		{
			var id = _repo.All.First(x => x.Name == args[0]).ItemId;
			_repo.Give(id);
		}
	}
}
