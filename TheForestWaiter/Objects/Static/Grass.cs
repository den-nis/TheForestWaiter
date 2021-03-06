using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entites;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Objects.Static
{
    class Grass : StaticObject
    {
        const int FRAME_RATE_MIN = 2;
        const int FRAME_RATE_MAX = 5;

        private int _cellsWide;

        AnimatedSprite[] Animations { get; set; }

        public Grass(GameData game) : base(game)
        {

        }
       
        private void SetupGrassFields()
		{
            Animations = new AnimatedSprite[_cellsWide];
            for (int i = 0; i < _cellsWide; i++)
            {
                Animations[i] = GameContent.Textures.CreateAnimatedSprite("Textures\\World\\grass.png");
                Animations[i].Framerate = (int)Rng.Range(FRAME_RATE_MIN, FRAME_RATE_MAX);
                Animations[i].CurrentFrame = (int)Rng.Range(Animations[i].AnimationStart, Animations[i].AnimationEnd);
            }
        }

        public override void Update(float time)
        {
            for (int i = 0; i < _cellsWide; i++)
            {
                Animations[i].Sheet.Sprite.Position = Position + new Vector2f(i * World.TILE_SIZE, -Animations[i].Sheet.TileSize.Y);
                Animations[i].Update(time);
            }

            base.Update(time);
        }

		public override void PrepareSpawn(MapObject obj)
		{
            _cellsWide = obj.Width / World.TILE_SIZE;
            Position = new Vector2f(obj.X, obj.Y);
            SetupGrassFields();
        }

		public override void Draw(RenderWindow window)
        {
            foreach (var animation in Animations)
            {
                window.Draw(animation);
            }
        }
    }
}
