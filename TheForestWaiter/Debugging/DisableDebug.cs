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
	class DisableDebug : IGameDebug
	{
		public void Draw(RenderWindow window) { }

		public void DrawHitBox(DynamicObject obj) { }

		public void DrawWorldCollision(Vector2f pos) { }

		public T GetVariable<T>(Variables name, T defaultValue) => defaultValue;

		public void Setup() { }

		public void Update(string command) { }
	}
}
