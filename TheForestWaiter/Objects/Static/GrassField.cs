using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entities;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Objects.Static
{
    class GrassField : StaticObject
    {
        private int _cellsWide;

        public GrassField(GameData game) : base(game)
        {
        }
       
        private void SetupGrassFields()
		{
            if (UserSettings.GetBool("Game", "EnableGrass"))
            {
                for (int i = 0; i < _cellsWide; i++)
                {
                    var grass = new Grass(Game);
                    grass.Position = Position + new Vector2f(i * World.TILE_SIZE, -grass.Size.Y);
                    Game.Objects.Chunks.Add(grass);
                }
            }

            MarkedForDeletion = true;
        }

		public override void PrepareSpawn(MapObject obj)
		{
            _cellsWide = obj.Width / World.TILE_SIZE;
            Position = new Vector2f(obj.X, obj.Y);
            SetupGrassFields();
        }

		public override void Draw(RenderWindow window)
		{
		}

		public override void Update(float time)
		{
		}
	}
}
