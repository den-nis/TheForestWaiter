using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TheForestWaiter.States;

namespace TheForestWaiter.Resources
{
    [Obsolete]
    public static class Provider
    {
        private static Dictionary<string, object> Resources { get; set; } = new Dictionary<string, object>();

        public static void Provide<T>(T resource)
        {
            Resources.Add(GetName<T>(), resource);
        }

        public static void ProvideOverwrite<T>(T resource)
        {
            if (Resources.ContainsKey(GetName<T>()))
                RemoveResource<T>();

            Provide(resource);
        }

        public static void RemoveResource<T>()
        {
            if (Resources[GetName<T>()] is IDisposable dispose)
                dispose.Dispose();

            Resources.Remove(GetName<T>());
        }

        public static T Request<T>() => (T)Resources[GetName<T>()];
       
        public static bool TryRequest<T>(out T resource)
        {
            resource = default;
            if (Resources.TryGetValue(GetName<T>(), out object data))
            {
                resource = (T)data;
                return true;
            }

            return false;
        }

        private static string GetName<T>() => typeof(T).FullName;
    }
}
