using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Debugging.Variables;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using LightInject;
using TheForestWaiter.States;

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

                .Register<Entry>()
                .Register<GameState>()

                .RegisterSingleton<GameContent>()
                .RegisterSingleton<GameVariables>()
                .RegisterSingleton<StateManager>()
                .RegisterSingleton<UserSettings>()
                .RegisterSingleton<WindowHandle>()
#if DEBUG
                .RegisterSingleton<IGameDebug, GameDebug>()
#else
                .RegisterSingleton<IGameDebug, DisableDebug>()
#endif
            ;
        }

        public void Setup()
        {
            _container.GetInstance<UserSettings>().Setup();
            _container.GetInstance<WindowHandle>().Setup();
            _container.GetInstance<IGameDebug>().Setup();
            _container.GetInstance<GameContent>().Setup();
        }
    }
}
