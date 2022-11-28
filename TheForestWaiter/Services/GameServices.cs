using LightInject;
using System;
using System.Diagnostics;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Gibs;
using TheForestWaiter.Game.Hud;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Items;
using TheForestWaiter.Game.Weapons;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.Multiplayer.Handlers;

namespace TheForestWaiter.Services
{
	internal class GameServices : IServices, IDisposable
	{
		private readonly ServiceContainer _container;
		private Scope _scope;

		public GameServices()
		{
			_container = IoC.GetInstance<ServiceContainer>();
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
				.RegisterScoped<ItemRepository>()
				.RegisterScoped<NetTraffic>()
				.RegisterScoped<GameMessages>()
				.Register<PickupSpawner>()
				.Register<DropSpawner>()
				.Register<GibSpawner>()
				.Register<Inventory>()

				//Weapons
				.Register<Handgun>()
				.Register<Shotgun>()
				.Register<Chaingun>()
				.Register<Sniper>()
				.Register<Bow>();

			var networking = IoC.GetInstance<NetSettings>();

			if (networking.IsMultiplayer)
				_container
					.RegisterScoped<NetContext>()
					.RegisterScoped<PlayerGhosts>()
					.RegisterScoped<SharedState>();

			if (networking.IsClient)
				_container.RegisterScoped<PackageHandler, ClientSidePackageHandler>();

			if (networking.IsHost)
				_container.RegisterScoped<PackageHandler, ServerSidePackageHandler>();
				_container.RegisterScoped<NetServer>();
		}

		private void RegisterAllGameObjects()
		{
			foreach (var obj in Types.GameObjects.Values)
			{
				_container.Register(obj);
			}
		}

		public void Dispose()
		{
			Debug.WriteLine("Disposing game services");
			_scope.Dispose();
		}

		public void Setup()
		{
			_container.GetInstance<GameController>().Setup();
			_container.GetInstance<NetTraffic>().Setup();
		}
	}
}
