using LightInject;

namespace TheForestWaiter
{
	internal class IoC
	{
		private static IServiceContainer _container;

		public static void SetContainer(ServiceContainer container)
		{
			_container = container;
			_container.RegisterSingleton<ServiceContainer>();
		}

		public static T GetInstance<T>()
		{
			return _container.GetInstance<T>();
		}
	}
}
