using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game;

namespace TheForestWaiter.Game.Debugging
{
	interface IGameDebug : IDisposable
	{
		void Setup();

		void Update(float time);

		void Draw(RenderWindow window);

		void DrawHitBox(Vector2f position, Vector2f size, Color color);

		void DrawWorldCollision(Vector2f pos);

		void Log(string message);
	}
}