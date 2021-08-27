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
	class GameDebug : IGameDebug
	{
		public void Draw(RenderWindow window)
		{
			throw new NotImplementedException();
		}

		public void DrawHitBox(DynamicObject obj)
		{
			throw new NotImplementedException();
		}

		public void DrawWorldCollision(Vector2f pos)
		{
			throw new NotImplementedException();
		}

		public T GetVariable<T>(Variables name, T defaultValue)
		{
			throw new NotImplementedException();
		}

		public void Setup()
		{
			throw new NotImplementedException();
		}

		public void Update(string command)
		{
			throw new NotImplementedException();
		}
	}
}
