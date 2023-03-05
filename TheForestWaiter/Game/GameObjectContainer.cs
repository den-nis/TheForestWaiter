using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Performance;

namespace TheForestWaiter.Game.Core
{
	sealed internal class GameObjectContainer<G> : IGameObjectContainer, IEnumerable<G> where G : GameObject
	{
		private ProfileCategory? _profilerCaptureDraw;
		private ProfileCategory? _profilerCaptureUpdate;

		private readonly List<G> _objects = new();

		public void Draw(RenderWindow window)
		{
			Profiling.Start(_profilerCaptureDraw ?? default);

			foreach (var i in this)
			{
				if (!i.DisableDraws)
					i.Draw(window);
			}

			Profiling.End(_profilerCaptureDraw ?? default);
		}

		public void Update(float time)
		{
			Profiling.Start(_profilerCaptureUpdate ?? default);

			foreach (var i in this)
			{
				if (!i.DisableUpdates)
					i.Update(time);
			}

			Profiling.End(_profilerCaptureUpdate ?? default);
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

		public void ConfigureProfiler(ProfileCategory draw, ProfileCategory update)
		{
			_profilerCaptureDraw = draw;
			_profilerCaptureUpdate = update;
		}
	}
}
