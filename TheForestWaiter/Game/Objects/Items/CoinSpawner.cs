using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Gibs;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class CoinSpawner
	{
		public float MaxVelocity { get; set; } = 150;
		public float MinVelocity { get; set; } = 100;

		private readonly GameData _game;
		private readonly ObjectCreator _creator;

		public CoinSpawner(GameData game, ObjectCreator creator)
		{
			_game = game;
			_creator = creator;
		}

		public void Spawn(Vector2f at)
		{
			var velocity = TrigHelper.FromAngleRad(Rng.Angle(), Rng.Range(MinVelocity, MaxVelocity));
			var coin = _creator.CreateAndShoot<Coin>(at, velocity);

			_game.Objects.Other.Add(coin);
		}

		public void SpawnAmount(Vector2f at, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Spawn(at);
			}
		}
	}
}
