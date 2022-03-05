using LightInject;
using System;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Gibs;
using TheForestWaiter.Game.Hud;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Items;
using TheForestWaiter.Game.Objects.Weapons;

namespace TheForestWaiter.Services
{
	internal class GameServices : IServices, IDisposable
    {
        private readonly ServiceContainer _container;
        private Scope _scope;

        public GameServices(ServiceContainer container)
        {
            _container = container;
         }

        public void Register()
        {
            RegisterAllGameObjects();

            _scope = _container.BeginScope();

            _container
                .RegisterScoped<GameController>()
                .RegisterScoped<Background>()
                .RegisterScoped<Camera>()
                .RegisterScoped<GameHud>()
                .RegisterScoped<GameData>()
                .RegisterScoped<GameObjects>()
                .RegisterScoped<World>()
                .RegisterScoped<ObjectCreator>()
                .RegisterScoped<GibSpawner>()
                .RegisterScoped<PickupSpawner>()
                .RegisterScoped<DropSpawner>()
                .RegisterScoped<ItemShop>()

                .Register<Handgun>()
                .Register<Sniper>();
        }

        private void RegisterAllGameObjects()
        {
            foreach(var obj in Types.GameObjects.Values)
            {
                _container.Register(obj);
            }
        }

        public void Dispose()
        {
            _scope.Dispose();   
        }

        public void Setup()
        {
            _container.GetInstance<GameController>(); 
        }
    }
}
