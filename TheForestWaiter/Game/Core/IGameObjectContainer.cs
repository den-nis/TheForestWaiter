using SFML.Graphics;

namespace TheForestWaiter.Game.Core
{
	internal interface IGameObjectContainer
    {
        public void Draw(RenderWindow window);

        public void Update(float time);

        public void Clear();

        public void CleanupMarkedForDeletion();
    }
}
