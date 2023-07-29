using Newtonsoft.Json;
using SFML.Graphics;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Hud;
using TheForestWaiter.Performance;
using TheForestWaiter.Services;
using TheForestWaiter.States;

namespace TheForestWaiter.Game
{
	class GameState : IState
	{
		private readonly GameServices _services;

		private readonly WindowHandle _window;
		private readonly Camera _camera;
		private readonly ContentSource _content;
		private readonly GameHud _hud;
		private readonly GameData _game;

		private const float CLEAN_UP_INTERVAL = 10;
		private float _cleanUpTimer = CLEAN_UP_INTERVAL;
		private readonly Background _background;

		public GameState()
		{
			_services = new GameServices();
			_services.Register();

			_game = IoC.GetInstance<GameData>();
			_camera = IoC.GetInstance<Camera>();
			_background = IoC.GetInstance<Background>();
			_hud = IoC.GetInstance<GameHud>();
			_window = IoC.GetInstance<WindowHandle>();
			_content = IoC.GetInstance<ContentSource>();
		}

		public void Dispose()
		{
			_services.Dispose();
		}

		public void Load()
		{
			_services.Setup();

			var map = LoadMap();
			_game.LoadFromMap(map);
			SetupCamera();

			_background.Horizon = _game.Objects.Player.Position.Y;
		}

		private Map LoadMap()
		{
			return JsonConvert.DeserializeObject<Map>(_content.Source.GetString("Worlds/test.json"));
		}

		private void SetupCamera()
		{
			_camera.BaseSize = _window.SfmlWindow.Size.ToVector2f();
			_camera.Center = _game.Objects.Player.Center;
		}

		public void Draw()
		{
			Profiling.Start(ProfileCategory.DrawGame);

			_window.SfmlWindow.SetView(_camera.GetView());
			_window.SfmlWindow.Clear(new Color(54, 26, 103));

			_background.Draw(_window.SfmlWindow);


			_game.World.Draw(_window.SfmlWindow, false);
			_game.Objects.Draw(_window.SfmlWindow);
			_game.World.Draw(_window.SfmlWindow, true);

			_hud.Draw(_window.SfmlWindow);

			_window.SfmlWindow.SetView(Camera.GetWindowView(_window.SfmlWindow));

			Profiling.End(ProfileCategory.DrawGame);
		}

		public void Update(float time)
		{
			Profiling.Start(ProfileCategory.UpdateGame);

			_background.Update();
			_background.SetOffset(_camera.Center);
			_background.UpdateSize((int)_window.SfmlWindow.Size.X, (int)_window.SfmlWindow.Size.Y);

			_cleanUpTimer -= time;
			if (_cleanUpTimer < 0)
			{
				_cleanUpTimer = CLEAN_UP_INTERVAL;
				_game.Objects.CleanUp();
			}

			_game.Objects.Update(time);
			_camera.FollowPlayer(_game.Objects.Player.Center);
			_camera.Update(time);

			Profiling.End(ProfileCategory.UpdateGame);
		}
	}
}
