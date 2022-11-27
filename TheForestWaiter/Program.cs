using LightInject;
using System.Globalization;
using TheForestWaiter.Services;

namespace TheForestWaiter
{
	internal class Program
	{
		static void Main(string[] args)
		{
			SetCulture();

			using ServiceContainer container = new()
			{
				PropertyDependencySelector = new DisablePropertyDependencies()
			};

			IoC.SetContainer(container);

			GlobalServices startup = new(container, args);

			startup.Register();
			startup.Setup();
			container.GetInstance<Entry>().Run();
		}

		static void SetCulture()
		{
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		}
	}
}
