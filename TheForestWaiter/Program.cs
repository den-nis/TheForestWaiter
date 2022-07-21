using LightInject;
using System.Globalization;
using TheForestWaiter.Services;

namespace TheForestWaiter
{
	internal class Program
	{
		static void Main()
		{
			SetCulture();

			using ServiceContainer container = new()
			{
				PropertyDependencySelector = new DisablePropertyDependencies()
			};

			IoC.SetContainer(container);

			GlobalServices startup = new(container);

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
