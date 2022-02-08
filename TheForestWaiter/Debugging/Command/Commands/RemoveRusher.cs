using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Objects.Enemies;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("removerusher", "")]
    internal class RemoveRusher : ICommand
    {
        private readonly GameData _game;

        public RemoveRusher(GameData game)
        {
            _game = game;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _game.Objects.Enemies.First(e => e is Rusher).MarkedForDeletion = true;
        }
    }
}
