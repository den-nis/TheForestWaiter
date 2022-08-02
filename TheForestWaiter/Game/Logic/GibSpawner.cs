using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Graphics;
using TheForestWaiter.Game.Objects;

namespace TheForestWaiter.Game.Gibs
{
	internal class GibSpawner
	{
		public float? GibRadius { get; set; }
		public float MaxVelocity { get; set; } = 300;
		public float MinVelocity { get; set; } = 100;

		private const string NO_SHEET_SET = "No sheet set for gib spawner";
		private SpriteSheet _sheet;
		private readonly GameData _game;
		private readonly ContentSource _content;
		private readonly float _gibMultiplier;

		public GibSpawner()
		{
			var settings = IoC.GetInstance<UserSettings>();
			_game = IoC.GetInstance<GameData>();
			_content = IoC.GetInstance<ContentSource>();

			_gibMultiplier = settings.GetFloat("Game", "GibMultiplier");
		}

		public void Setup(string sheetName)
		{
			_sheet = _content.Textures.CreateSpriteSheet(sheetName);
		}

		public void SpawnAll(Vector2f at)
		{
			if (_sheet == null) throw new InvalidOperationException(NO_SHEET_SET);

			for (int i = 0; i < _sheet.TotatlTiles * _gibMultiplier; i++)
			{
				Spawn(at, i % _sheet.TotatlTiles);
			}
		}

		public void Spawn(Vector2f at, int index)
		{
			if (_sheet == null) throw new InvalidOperationException(NO_SHEET_SET);

			var velocity = TrigHelper.FromAngleRad(Rng.Angle(), Rng.Range(MinVelocity, MaxVelocity));
			var gib = new Gib(_sheet);

			gib.SetLife(10);
			gib.Center = at;
			gib.Velocity = velocity;
			gib.AngularMomentum = Math.Sign(velocity.X) * (float)Math.PI;
			gib.TileIndex = index;
			gib.Sheet = _sheet;

			_game.Objects.Other.Add(gib);
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
