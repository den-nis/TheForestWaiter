using LightInject;
using SFML.System;
using System;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Objects.Weapons.Abstract;

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

        public T CreateWeapon<T>() where T : Weapon =>  _provider.GetInstance<T>();

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

        public T FireProjectile<T>(Vector2f position, Vector2f speed, Creature owner) where T : Projectile
        {
            var bullet = CreateAndShoot<T>(position, speed);
            bullet.Claim(owner);
            return bullet;
		}
    }
}
