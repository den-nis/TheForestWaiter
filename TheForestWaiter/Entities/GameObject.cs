using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Environment;

namespace TheForestWaiter.Entites
{
    abstract class GameObject : IUpdateDraw
    {
        protected GameData Game { get; set; }

        public GameObject(GameData game)
        {
            Game = game;
        }

        private bool _MarkedForDeletion = false;
        public bool MarkedForDeletion
        {
            get => _MarkedForDeletion;
            set => _MarkedForDeletion |= value;
        }

        /// <summary>
        /// Can be used for setup logic that needs the tiled object data
        /// </summary>
        public virtual void PrepareSpawn(MapObject mapObject)
        {
        }

        public virtual void SetSpawn(Vector2f position)
		{
            Position = position;
            Position += new Vector2f(0, -Size.Y);
        }

        private static long IdCounter { get; set; } = 0;
        public long GameObjectId { get; } = IdCounter++;

        public Vector2f Position { get; set; }
        public Vector2f Size { get; set; }

        public Vector2f Center { get => Position + Size / 2; set => Position = value - Size / 2; }
        public IntRect IntRect => new IntRect(new Vector2i((int)Position.X, (int)Position.Y), new Vector2i((int)Size.X, (int)Size.Y));
        public FloatRect FloatRect => new FloatRect(Position, Size);

        public bool Intersects(GameObject other) => FloatRect.Intersects(other.FloatRect);

        public abstract void Draw(RenderWindow window);

        public abstract void Update(float time);
    }
}
