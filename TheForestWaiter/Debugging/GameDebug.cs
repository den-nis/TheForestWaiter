using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging.Command;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Debugging
{
	class GameDebug : IGameDebug
	{
        private GameData _game;
		private CommandHandler _commandHandler;

        public void Draw(RenderWindow window)
		{
		}

		public void DrawHitBox(Vector2f position, Vector2f size, Color color)
		{
		}

		public void DrawWorldCollision(Vector2f pos)
		{
		}

		public void Setup()
        {
            _commandHandler = new CommandHandler();
			_commandHandler.IndexAndStartConsoleThread();
        }

        public void Update(float time)
		{
			_commandHandler.Update();
		}

        public void ProvideGameData(GameData game)
        {
			_game = game;
			_commandHandler.ProvideGameData(game);
		}

        public void Dispose()
        {
            
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
