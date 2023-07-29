using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Environment.Spawning;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class WaveHud : HudSection
	{
		private readonly SpriteFont _waveText;
		private readonly GameData _game;
		private SpawnScheduler _scheduler;

		public WaveHud(float scale) : base(scale)
		{
			var content = IoC.GetInstance<ContentSource>();
			_game = IoC.GetInstance<GameData>();
			_scheduler = IoC.GetInstance<SpawnScheduler>();

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
			_waveText.SetText(_scheduler.WaveNumber.ToString());

			_waveText.Draw(window);
		}

		public override bool IsMouseOnAnyButton() => false;
		public override void OnMouseMove(Vector2i mouse) { }
		public override void OnPrimaryReleased() { }
		public override void OnPrimaryPressed() { }
	}
}
