using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entities;

namespace TheForestWaiter.Debugging
{
	interface IGameDebug
	{
		void Setup();

		void Update(string command);

		void Draw(RenderWindow window);

		T GetVariable<T>(Variables name, T defaultValue);

		void DrawHitBox(DynamicObject obj);

		void DrawWorldCollision(Vector2f pos);
	}
}