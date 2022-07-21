using SFML.Graphics;
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
