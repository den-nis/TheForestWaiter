using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game;
using TheForestWaiter.Debugging;

namespace TheForestWaiter.Game.Debugging
{
	interface IDebug : IDisposable
	{
		void Setup();

		void Update(float time);

		void Draw(RenderWindow window);

		void Log(string message);
	}
}