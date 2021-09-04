using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using LightInject;
using TheForestWaiter.States;
using System.Diagnostics;
using TheForestWaiter.Debugging.Command;
using System.Reflection;

namespace TheForestWaiter.Services
{
    class GlobalServices : IServices
    {
        private readonly ServiceContainer _container;

        public GlobalServices(ServiceContainer container)
        {
            _container = container;
        }

        public void Register()
        {
            _container
                .RegisterInstance(_container)
                .RegisterInstance<IServiceContainer>(_container)

                .Register<GameState>()

                .RegisterSingleton<Entry>()
                .RegisterSingleton<GameContent>()
                .RegisterSingleton<StateManager>()
                .RegisterSingleton<UserSettings>()
                .RegisterSingleton<WindowHandle>()
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
            _container.GetInstance<WindowHandle>().Setup();
            _container.GetInstance<IDebug>().Setup();
            _container.GetInstance<GameContent>().Setup();
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
