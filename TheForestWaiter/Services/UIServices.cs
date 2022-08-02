using LightInject;
using System;
using System.Diagnostics;
using TheForestWaiter.Game;

namespace TheForestWaiter.Services
{
	internal class UIServices : IServices, IDisposable
	{
		private readonly ServiceContainer _container;
		private Scope _scope;

		public UIServices()
		{
			_container = IoC.GetInstance<ServiceContainer>();
		}

		public void Register()
		{
			_scope = _container.BeginScope();

			_container
				.RegisterScoped<UIController>();
		}

		public void Dispose()
		{
			Debug.WriteLine("Disposing UI services");
			_scope.Dispose();
		}

		public void Setup()
		{
			_container.GetInstance<UIController>().Setup();
		}
	}
}
