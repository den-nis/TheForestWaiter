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

        public GameObject CreateType(Type type) => (GameObject)_provider.GetInstance(type);

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
            obj.Velocity = speed;
            return obj;
        }
    }
}
