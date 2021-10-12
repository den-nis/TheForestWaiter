using System.Globalization;
using TheForestWaiter.Game;
using LightInject;
using TheForestWaiter.Services;

namespace TheForestWaiter
{
    class Program
    {
        static void Main()
        {
            SetCulture();

            using ServiceContainer services = new()
            {
                PropertyDependencySelector = new DisablePropertyDependencies()
            };

            GlobalServices startup = new(services);

            startup.Register();
            startup.Setup();
            services.GetInstance<Entry>().Run();        
        }

        static void SetCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}
