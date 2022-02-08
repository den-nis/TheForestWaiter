using SFML.System;
using System;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Gibs
{
	internal class GibSpawner
	{
		private const float PERCENTAGE_TAKE_INITIAL_VELOCITY = 0.3f;

		public SpriteSheet Sheet { get; set; }
		public float? GibRadius { get; set; }
		public float MaxVelocity { get; set; } = 700;
		public float MinVelocity { get; set; } = 200;
		public Vector2f InitialVelocity { get; set; } = new Vector2f(0, 0);

		private readonly GameData _game;
        private readonly ObjectCreator _creator;
        private readonly float _gibMultiplier;

		public GibSpawner(GameData game, UserSettings settings, ObjectCreator creator)
		{
			_gibMultiplier = settings.GetFloat("Game", "GibMultiplier"); //TODO: type safe
			_game = game;
            _creator = creator;
        }

		public void Spawn(Vector2f at, int index)
		{
			var velocity = TrigHelper.FromAngleRad(Rng.Angle(), Rng.Range(MinVelocity, MaxVelocity));

			var gib = _creator.CreateAndShoot<Gib>(at, velocity + InitialVelocity * PERCENTAGE_TAKE_INITIAL_VELOCITY);
            gib.SetLife(10);
			gib.AngularMomentum = Math.Sign(velocity.X) * (float)Math.PI;
			gib.TileIndex = index;
			gib.Sheet = Sheet;

			_game.Objects.Other.Add(gib);
		}

		public void SpawnAll(Vector2f at)
		{
			for (int i = 0; i < Sheet.TotatlTiles * _gibMultiplier; i++)
			{
				Spawn(at, i % Sheet.TotatlTiles);
			}
		}

		public void SpawnAmount(Vector2f at, int count)
		{
			for (int i = 0; i < count * _gibMultiplier; i++)
			{
				Spawn(at, (int)Math.Round(Rng.Range(0, Sheet.TotatlTiles - 1)));
			}
		}
	}
}
