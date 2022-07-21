using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Core
{
	sealed internal class GameObjectContainer<G> : IGameObjectContainer, IEnumerable<G> where G : GameObject
	{
		private readonly List<G> _objects = new();

		public void Draw(RenderWindow window)
		{
			foreach (var i in this)
			{
				if (!i.DisableDraws)
					i.Draw(window);
			}
		}

		public void Update(float time)
		{
			foreach (var i in this)
			{
				if (!i.DisableUpdates)
					i.Update(time);
			}
		}

		public void Add(G obj)
		{
			_objects.Add(obj);
		}

		public void Clear() => _objects.Clear();

		public void CleanupMarkedForDeletion()
		{
			foreach (var delete in _objects.Where(o => o.MarkedForDeletion && o is IDisposable))
			{
				((IDisposable)delete).Dispose();
			}

			_objects.RemoveAll(o => o.MarkedForDeletion);
		}

		public IEnumerator<G> GetEnumerator() => _objects.Where(o => !o.MarkedForDeletion).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
