using LightInject;

namespace TheForestWaiter
{
	internal class IoC
	{
		private static IServiceContainer _container;

		public static void SetContainer(IServiceContainer container)
		{
			_container = container;
		}

		public static T GetInstance<T>()
		{
			return _container.GetInstance<T>();
		}
	}
}
