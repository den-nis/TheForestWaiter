using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Gibs
{
	class GibSpawner
	{
		public float? GibRadius { get; set; }

		public Vector2f MaxVelocity { get; set; } = new Vector2f(300, 0);
		public Vector2f MinVelocity { get; set; } = new Vector2f(-300, -200);

		private readonly GameData _game;
		private readonly SpriteSheet _sheet;
		private readonly float _gibMultiplier;

		public GibSpawner(GameData game, SpriteSheet sheet)
		{
			_gibMultiplier = UserSettings.GetFloat("Game", "GibMultiplier");
			_game = game;
			_sheet = sheet;
		}

		public void Spawn(Vector2f at, int index)
		{
			var velocity = new Vector2f(
				Rng.Range(MinVelocity.X, MaxVelocity.X),
				Rng.Range(MinVelocity.Y, MaxVelocity.Y)
				);

			var gib = new Gib(_game, _sheet, index, 10, Math.Sign(velocity.X) * (float)Math.PI)
			{
				velocity = velocity,
				Position = at,
			};

			_game.Objects.Other.Add(gib);
		}

		public void SpawnComplete(Vector2f at)
		{
			for (int i = 0; i < _sheet.TotatlTiles * _gibMultiplier; i++)
			{
				Spawn(at, i % _sheet.TotatlTiles);
			}
		}

		public void SpawnAmount(Vector2f at, int count)
		{
			for (int i = 0; i < count * _gibMultiplier; i++)
			{
				Spawn(at, (int)Math.Round(Rng.Range(0, _sheet.TotatlTiles - 1)));
			}
		}
	}
}
