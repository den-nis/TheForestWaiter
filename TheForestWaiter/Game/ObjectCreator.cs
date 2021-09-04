using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Objects.Weapons;
using LightInject;

namespace TheForestWaiter.Game
{
    class ObjectCreator
    {
        private readonly IServiceContainer _provider;

        public ObjectCreator(IServiceContainer provider)
        {
            _provider = provider;
        }

        //TODO: remove this method
        public object CreateType(Type type) => _provider.GetInstance(type);

        public T CreateGun<T>() where T : GunBase => _provider.GetInstance<T>();
        
        public T Create<T>() where T : GameObject => _provider.GetInstance<T>();
        
        public T CreateAt<T>(Vector2f position) where T : GameObject
        {
            T obj = _provider.GetInstance<T>();
            obj.Center = position;
            return obj;
        }

        public T CreateAndShoot<T>(Vector2f position, Vector2f speed) where T : PhysicsObject
        {
            T obj = CreateAt<T>(position);
            obj.velocity = speed;
            return obj;
        }

        //TODO: explain
        public T CreateAbove<T>(Vector2f position) where T : GameObject
        {
            T obj = _provider.GetInstance<T>();
            obj.Position = position + new Vector2f(0, -obj.Size.Y);
            return obj;
        }
    }
}
