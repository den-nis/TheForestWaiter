using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Game.Environment;

namespace TheForestWaiter.Game.Objects.Abstract
{
	internal abstract class GameObject 
    {
        private static long _idCounter = 0;
        public long GameObjectId { get; } = _idCounter++;

        public Vector2f Position { get; set; }
        public Vector2f Size { get; set; }
        public Vector2f Center { get => Position + Size / 2; set => Position = value - Size / 2; }

        public IntRect IntRect => new(new Vector2i((int)Position.X, (int)Position.Y), new Vector2i((int)Size.X, (int)Size.Y));
        public FloatRect FloatRect => new(Position, Size);

        private bool _markedForDeletion = false;
        public bool MarkedForDeletion => _markedForDeletion;

        public bool DisableUpdates { get; set; }
        public bool DisableDraws { get; set; }

        protected GameData Game { get; set; }

        public GameObject(GameData game)
        {
            Game = game;
        }

        public bool Intersects(GameObject other) => FloatRect.Intersects(other.FloatRect);

        public abstract void Draw(RenderWindow window);

        public abstract void Update(float time);

        public virtual void DrawHitbox(RenderWindow window, float lineThickness)
        {
            window.DrawHitBox(Position, Size, Color.Green, lineThickness);
        }

        public void Delete() => _markedForDeletion = true;

        /// <summary>
        /// Can be used for setup logic that needs the tiled object data
        /// </summary>
        public virtual void MapSetup(MapObject mapObject)
        {

        }
    }
}
