using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Objects.Weapons;
using LightInject;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Projectiles;

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

        public T CreateAndShoot<T>(Vector2f position, Vector2f speed) where T : Movable
        {
            T obj = CreateAt<T>(position);
            obj.SetVelocityX(speed.X);
            obj.SetVelocityY(speed.Y);
            return obj;
        }

        public T FireBullet<T>(Vector2f position, Vector2f speed, Creature owner) where T : Bullet
        {
            var bullet = CreateAndShoot<T>(position, speed);
            bullet.Owner = owner;
            return bullet;
		}
    }
}
