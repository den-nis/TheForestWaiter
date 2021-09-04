using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("hitbox", "Toggle hitboxes")]
    class HitBoxes : ICommand
    {
        private readonly GameData _game;

        public HitBoxes(IServiceContainer provider, GameData game)
        {
            _game = game;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _game.Objects.EnableDrawHitBoxes = !_game.Objects.EnableDrawHitBoxes;
            Console.WriteLine(_game.Objects.EnableDrawHitBoxes ? "Enabled hitboxes" : "Disabled hitboxes");
        }
    }
}
