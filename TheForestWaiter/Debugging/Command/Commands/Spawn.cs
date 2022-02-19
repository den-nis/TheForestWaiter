using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging.DebugConsole;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Objects;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("spawn", "Spawn an object", "{name} ?{amount} ?{x} ?{y}")]
    class Spawn : ICommand
    {
        private readonly IServiceContainer _container;
        private readonly GameData _game;

        public Spawn(IServiceContainer provider, GameData game)
        {
            _container = provider;
            _game = game;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            int amount = args.Length > 1 ? int.Parse(args[1]) : 1;

            var pos = _game.Objects.Player.Center;

            if (args.Length > 2)
                pos.X = args[2] == "*" ? pos.X : int.Parse(args[2]); 

            if (args.Length > 3)
                pos.Y = args[3] == "*" ? pos.Y : int.Parse(args[3]);

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
