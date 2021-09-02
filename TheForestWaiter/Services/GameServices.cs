using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Environment;
using LightInject;
using TheForestWaiter.Game.Objects.Static;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Weapons.Guns;
using TheForestWaiter.Game.Objects.Spawners;
using TheForestWaiter.Game.Objects.Weapons.Bullets;
using TheForestWaiter.Game.Entities;
using System.Reflection;
using TheForestWaiter.Game.Gibs;

namespace TheForestWaiter.Services
{
    class GameServices : IServices, IDisposable
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
                .RegisterScoped<GameData>()
                .RegisterScoped<GameObjects>()
                .RegisterScoped<World>()
                .RegisterScoped<ObjectCreator>()
                .RegisterScoped<GibSpawner>()

                .Register<Handgun>()
                .Register<Sniper>();
        }

        public void RegisterAllGameObjects()
        {
            var asm = Assembly.GetExecutingAssembly();
            var objects = asm.GetTypes().Where(t => t.IsAssignableTo(typeof(GameObject)));

            foreach(var obj in objects)
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
