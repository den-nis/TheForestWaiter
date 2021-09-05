using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Weapons;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("equip", "change current weapon", "{gun name}")]
    class Equip : ICommand
    {
        private readonly GameData _game;
        private readonly IServiceContainer _container;

        public Equip(GameData game, IServiceContainer container)
        {
            _game = game;
            _container = container;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _game.Objects.Player.SetGun((GunBase)_container
                .GetInstance(Types.Guns.FirstOrDefault(t => t.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase))));
        }
    }
}
