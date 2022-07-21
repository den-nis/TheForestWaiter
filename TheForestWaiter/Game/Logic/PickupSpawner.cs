using SFML.System;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class PickupSpawner
	{
		public float MAX_VELOCITY { get; set; } = 150;
		public float MIN_VELOCITY { get; set; } = 100;

		private readonly GameData _game;
		private readonly ObjectCreator _creator;

		public PickupSpawner()
		{
			_game = IoC.GetInstance<GameData>();
			_creator = IoC.GetInstance<ObjectCreator>();
		}

		public void SpawnAmount(Vector2f at, int coins, int hearts)
		{
			var velocity = TrigHelper.FromAngleRad(Rng.Angle(), Rng.Range(MIN_VELOCITY, MAX_VELOCITY));

			for (int i = 0; i < coins; i++)
			{
				_game.Objects.AddGameObject(_creator.CreateAndShoot<Coin>(at, velocity));
			}

			for (int i = 0; i < hearts; i++)
			{
				_game.Objects.AddGameObject(_creator.CreateAndShoot<Apple>(at, velocity));
			}
		}
	}
}
