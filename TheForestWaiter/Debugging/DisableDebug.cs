using SFML.Graphics;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Debugging
{
	class DisableDebug : IDebug
	{
		public void Update(float time) { }

		public void Draw(RenderWindow window) { }

		public void Setup(string[] args) { }

		public void Dispose() { }

		public void Log(string message) { }

		public void LogNetworking(string message) { }
	}
}
