using LightInject;
using SFML.System;
using System;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Weapons.Abstract;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter.Game
{
	internal class ObjectCreator
	{
		private readonly IServiceContainer _provider;

		public ObjectCreator(IServiceContainer provider)
		{
			_provider = provider;
		}

		public GameObject CreateType(Type type) => (GameObject)_provider.GetInstance(type);

		public T CreateWeapon<T>(Creature owner) where T : Weapon
		{
			var weapon = _provider.GetInstance<T>();
			weapon.Owner = owner;
			return weapon;
		} 

		public T Create<T>() where T : GameObject => _provider.GetInstance<T>();

		public T CreateAt<T>(Vector2f position) where T : GameObject => (T)CreateAt(typeof(T), position);

		public GameObject CreateAt(Type type, Vector2f position)
		{
			GameObject obj = CreateType(type);
			obj.Center = position;
			return obj;
		}

		public T CreateAndShoot<T>(Vector2f position, Vector2f speed) where T : Movable => (T)CreateAndShoot(typeof(T), position, speed);

		public Movable CreateAndShoot(Type type, Vector2f position, Vector2f speed)
		{
			Movable obj = (CreateAt(type, position) as Movable) ?? throw new InvalidOperationException("Unexpected gameobject type");
			obj.SetVelocityX(speed.X);
			obj.SetVelocityY(speed.Y);
			return obj;
		}

		public T FireProjectile<T>(Vector2f position, Vector2f speed, Creature owner) where T : Projectile =>
			(T)FireProjectile(typeof(T), position, speed, owner);

		public Projectile FireProjectile(Type type, Vector2f position, Vector2f speed, Creature owner)
		{
			var bullet = (CreateAndShoot(type, position, speed) as Projectile) ?? throw new InvalidOperationException("Unexpected gameobject type");
			bullet.Claim(owner);
			return bullet;
		}
	}
}
