using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Objects.Weapons;

namespace TheForestWaiter
{
    public static class Types
    {
        public static IDictionary<string, Type> GameObjects { get; private set; }
        public static IDictionary<string, Type> Guns { get; private set; }

        static Types()
        {
            GameObjects = GetGameObjects();
            Guns = GetGuns();
        }

        private static IDictionary<string, Type> GetGameObjects() => GetTypes<GameObject>();
        
        private static IDictionary<string, Type> GetGuns() => GetTypes<GunBase>();
        
        private static IDictionary<string, Type> GetTypes<T>()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(t =>
                t.IsAssignableTo(typeof(T)) &&
                !t.IsAbstract &&
                !t.IsInterface)
                .ToDictionary(k => k.Name, v => v);
        }
    }
}
