using SFML.System;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
	[Command("setpos", "Set the position of the player", "{x} {y}")]
    internal class SetPos : ICommand
    {
        private readonly GameData _game;

        public SetPos(GameData game)
        {
            _game = game;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _game.Objects.Player.Position = new Vector2f(float.Parse(args[0]), float.Parse(args[1]));
        }
    }
}
