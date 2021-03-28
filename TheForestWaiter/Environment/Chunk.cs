using SFML.Graphics;
using TheForestWaiter.Entities;

namespace TheForestWaiter.Environment
{
    class Chunk
    {
        public GameObjectContainer<StaticObject> Objects { get; } = new GameObjectContainer<StaticObject>();

        public void Draw(RenderWindow window)
        {
            Objects.Draw(window);
        }

        public void Update(float time)
        {
            Objects.Update(time);
        }

        public void Unloaded()
        {
            Objects.CleanupMarkedForDeletion();
        }
    }
}
