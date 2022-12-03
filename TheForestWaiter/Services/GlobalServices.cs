using LightInject;
using System.Linq;
using System.Reflection;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Debugging.Command;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.States;
using TheForestWaiter.UI.Menus;

namespace TheForestWaiter.Services
{
	class GlobalServices : IServices
	{
		private readonly ServiceContainer _container;
		private readonly string[] _args;

		public GlobalServices(ServiceContainer container, string[] args)
		{
			_container = container;
			_args = args;
		}

		public void Register()
		{
			_container
				.RegisterInstance(_container)
				.RegisterInstance<IServiceContainer>(_container)

				.Register<MainMenu>()
				.Register<CreditsMenu>()
				.Register<GameState>()

				.RegisterSingleton<Entry>()
				.RegisterSingleton<ContentSource>()
				.RegisterSingleton<StateManager>()
				.RegisterSingleton<UserSettings>()
				.RegisterSingleton<WindowHandle>()
				.RegisterSingleton<TimeProcessor>()
				.RegisterSingleton<SoundSystem>()
				.RegisterSingleton<PlayStats>()
				.RegisterSingleton<NetSettings>()
				.RegisterSingleton<NetContext>()
				

				//Services
				.Register<UIServices>()
				.Register<GameServices>()

#if DEBUG
				.RegisterSingleton<IDebug, ActiveDebug>();
			RegisterCommands();
#else
                .RegisterSingleton<IDebug, DisableDebug>();
#endif
		}

		public void Setup()
		{
			_container.GetInstance<UserSettings>().Setup();
			_container.GetInstance<ContentSource>().Setup();
			_container.GetInstance<WindowHandle>().Setup();
			_container.GetInstance<IDebug>().Setup(_args);
		}

		private void RegisterCommands()
		{
			var asm = Assembly.GetExecutingAssembly();
			var commands = asm.GetTypes().Where(t => t.IsAssignableTo(typeof(ICommand)));

			foreach (var command in commands)
			{
				_container.Register(command);
			}
		}
	}
}
