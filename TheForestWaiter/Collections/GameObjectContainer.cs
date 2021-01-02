using SFML.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheForestWaiter.Entites;
using TheForestWaiter.Debugging;

namespace TheForestWaiter
{
    public class GameObjectContainer<G> : IEnumerable<G>, IUpdateDraw where G : GameObject
    {
        List<G> Objects { get; set; } = new List<G>();

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
            Objects.Add(obj);
        }

        public void Clear() => Objects.Clear();

        public void CleanupMarkedForDeletion()
        {
            GameDebug.LogDeletions<G>(Objects.Where(o => o.MarkedForDeletion));

            Objects.RemoveAll(o => o.MarkedForDeletion);
        }

        public IEnumerator<G> GetEnumerator() => Objects.Where(o => !o.MarkedForDeletion).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
