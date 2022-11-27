using SFML.Graphics;
using System;

namespace TheForestWaiter.Game.Debugging
{
	interface IDebug : IDisposable
	{
		void Setup(string[] args);

		void Update(float time);

		void Draw(RenderWindow window);

		void Log(string message);

		void LogNetworking(string message);
	}
}