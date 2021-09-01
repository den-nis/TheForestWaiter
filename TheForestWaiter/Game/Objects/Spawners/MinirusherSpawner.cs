using SFML.Graphics;
using System;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Objects.Enemies;

namespace TheForestWaiter.Game.Objects.Spawners
{
	//TODO: probably not needed
	class MinirusherSpawner : SpawnerBase
	{
        private readonly ObjectCreator _creator;

        public MinirusherSpawner(GameData game, ObjectCreator creator) : base(game)
		{
            _creator = creator;
        }

		public override void Draw(RenderWindow window)
		{

		}

		protected override void OnFirstUpdate()
		{
			Game.Objects.Enemies.Add(_creator.CreateAt<Minirusher>(Position));
		}
	}
}
