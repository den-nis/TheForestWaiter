using SFML.System;
using System;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Gibs
{
	class GibSpawner
	{
		public SpriteSheet Sheet { get; set; }
		public float? GibRadius { get; set; }
		public Vector2f MaxVelocity { get; set; } = new Vector2f(300, 0);
		public Vector2f MinVelocity { get; set; } = new Vector2f(-300, -200);
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
			var velocity = new Vector2f(
				Rng.Range(MinVelocity.X, MaxVelocity.X),
				Rng.Range(MinVelocity.Y, MaxVelocity.Y)
				);

			var gib = _creator.CreateAndShoot<Gib>(at, velocity + InitialVelocity);
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
