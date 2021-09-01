using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Entities;

namespace TheForestWaiter.Game.Objects.Spawners
{
	abstract class SpawnerBase : StaticObject
	{
		public bool _initialUpdate = false;

		public SpawnerBase(GameData game) : base(game)
		{
			
		}

		public override void Update(float time)
		{
			if (!_initialUpdate)
				OnFirstUpdate();
			_initialUpdate = true;
		}

		protected abstract void OnFirstUpdate();
	}
}
