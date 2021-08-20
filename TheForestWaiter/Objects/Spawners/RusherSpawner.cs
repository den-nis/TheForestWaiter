using SFML.Graphics;
using System;
using TheForestWaiter.Objects.Enemies;

namespace TheForestWaiter.Objects.Spawners
{
	class RusherSpawner : SpawnerBase
	{
		public RusherSpawner(GameData game) : base(game)
		{

		}

		public override void Draw(RenderWindow window)
		{
		}

		protected override void OnFirstUpdate()
		{
			var rusher = new Rusher(Game);
			rusher.SetSpawn(Position);
			Game.Objects.Enemies.Add(rusher);
		}
	}
}
