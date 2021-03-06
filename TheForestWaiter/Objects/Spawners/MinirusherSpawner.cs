using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entites;
using TheForestWaiter.Objects.Enemies;

namespace TheForestWaiter.Objects.Spawners
{
	class MinirusherSpawner : SpawnerBase
	{
		public MinirusherSpawner(GameData game) : base(game)
		{

		}

		protected override void OnFirstUpdate()
		{
			var rusher = new Minirusher(Game);
			rusher.SetSpawn(Position);
			Game.Objects.Enemies.Add(rusher);
		}
	}
}
