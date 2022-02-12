using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Gibs;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class PickupSpawner
	{
		public float MAX_VELOCITY { get; set; } = 150;
		public float MIN_VELOCITY { get; set; } = 100;

		private readonly GameData _game;
		private readonly ObjectCreator _creator;

		public PickupSpawner(GameData game, ObjectCreator creator)
		{
			_game = game;
			_creator = creator;
		}

		public void SpawnAmount(Vector2f at, int count, float percentageOfCoins)
		{
			var velocity = TrigHelper.FromAngleRad(Rng.Angle(), Rng.Range(MIN_VELOCITY, MAX_VELOCITY));

			for (int i = 0; i < count; i++)
			{
				PhysicsObject obj;
				if (Rng.Float() < percentageOfCoins)
				{
					obj = _creator.CreateAndShoot<Coin>(at, velocity);
				}
				else
				{
					obj = _creator.CreateAndShoot<Apple>(at, velocity);
				}

				_game.Objects.AddGameObject(obj);
			}
		}
	}
}
