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
            SetCulture("en");
            using ServiceContainer services = new()
            {
                PropertyDependencySelector = new DisablePropertyDependencies()
            };

            GlobalServices startup = new(services);

            startup.Register();
            startup.Setup();

            services.GetInstance<Entry>().Run();        
        }

        static void SetCulture(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
        }
    }
}
