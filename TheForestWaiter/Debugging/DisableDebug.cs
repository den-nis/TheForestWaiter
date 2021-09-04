using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Debugging
{
	class DisableDebug : IDebug
	{
        public void Update(float time) { }

		public void Draw(RenderWindow window) { }

		public void Setup() { }

        public void Dispose() { }

		public void Log(string message) { }
    }
}
