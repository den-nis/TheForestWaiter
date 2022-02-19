using System;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("god", "")]
    class GodMode : ICommand
    {
        private readonly GameData _game;

        public GodMode(GameData game)
        {
            _game = game;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _game.Objects.Player.God();
        }
    }
}
