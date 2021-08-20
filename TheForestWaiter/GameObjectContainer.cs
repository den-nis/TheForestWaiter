﻿using SFML.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Entities;
using TheForestWaiter.Debugging;

namespace TheForestWaiter
{
    class GameObjectContainer<G> : IEnumerable<G> where G : GameObject
    {
        private readonly List<G> _objects = new();

        public void Draw(RenderWindow window)
        {
            foreach(var i in this)
            {
                i.Draw(window);
            }
        }

        public void Update(float time)
        {
            foreach (var i in this)
            {
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
            GameDebug.LogDeletions<G>(_objects.Where(o => o.MarkedForDeletion));

            _objects.RemoveAll(o => o.MarkedForDeletion);
        }

        public IEnumerator<G> GetEnumerator() => _objects.Where(o => !o.MarkedForDeletion).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}