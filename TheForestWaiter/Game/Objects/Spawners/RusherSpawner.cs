using SFML.Graphics;
using System;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Objects.Enemies;

namespace TheForestWaiter.Game.Objects.Spawners
{
	//TODO: probably not needed
	class RusherSpawner : SpawnerBase
	{
        private readonly ObjectCreator _creator;

        public RusherSpawner(GameData game, ObjectCreator creator) : base(game)
		{
            this._creator = creator;
        }

		public override void Draw(RenderWindow window)
		{
		}

		protected override void OnFirstUpdate()
		{
			Game.Objects.Enemies.Add(_creator.CreateAt<Rusher>(Position));
		}
	}
}
