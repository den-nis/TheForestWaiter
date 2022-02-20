using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Static
{
	internal class GrassField : Immovable
	{
		private int _cellsWide;

		private readonly bool _enabled;
		private readonly ObjectCreator _creator;

		public GrassField(GameData game, UserSettings settings, ObjectCreator creator) : base(game)
		{
			_enabled = settings.GetBool("Game", "EnableGrass");
			_creator = creator;
		}

		private void SetupGrassFields()
		{
			if (_enabled)
			{
				for (int i = 0; i < _cellsWide; i++)
				{
					var grass = _creator.Create<Grass>();
					grass.Position = Position + new Vector2f(i * World.TILE_SIZE, -grass.Size.Y);
					Game.Objects.Environment.Add(grass);
				}
			}

			Delete();
		}

		public override void MapSetup(MapObject obj)
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
