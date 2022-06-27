using SFML.Graphics;
using SFML.System;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Objects.Static;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class WaveHud : HudSection
	{
		private readonly SpriteFont _waveText;
		private readonly GameData _game;
		private Spawner _spawner;

		public WaveHud(float scale, GameData game, ContentSource content) : base(scale)
		{
			_game = game;

			var numberSheet = content.Textures.CreateSpriteSheet("Textures/Hud/wave_numbers.png");
			_waveText = new SpriteFont(numberSheet);

			Size = numberSheet.TileSize
				.ToVector2f()
				.Multiply(new Vector2f(3, 1));
		}

		public override void Draw(RenderWindow window)
		{
			_waveText.Position = GetPosition(window);
			_waveText.Scale = Scale;
			_waveText.SetText(GetWaveNumber().ToString());

			_waveText.Draw(window);
		}

		private int GetWaveNumber()
		{
			if (_spawner == null)
				_spawner = (Spawner)_game.Objects.Environment.First(x => x is Spawner);

			return _spawner?.GetCurrentWave() ?? 0;
		}

		public override bool IsMouseOnAnyButton() => false;
        public override void OnMouseMove(Vector2i mouse) { }
		public override void OnPrimaryReleased() { }
        public override void OnPrimaryPressed() { }
    }
}
