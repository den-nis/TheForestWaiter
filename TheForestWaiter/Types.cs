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
        public static IEnumerable<Type> GameObjects { get; private set; }
        public static IEnumerable<Type> Guns { get; private set; }

        static Types()
        {
            GameObjects = GetGameObjects();
            Guns = GetGuns();
        }

        private static IEnumerable<Type> GetGameObjects() => GetTypes<GameObject>();
        
        private static IEnumerable<Type> GetGuns() => GetTypes<GunBase>();
        
        private static IEnumerable<Type> GetTypes<T>()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(t =>
                t.IsAssignableTo(typeof(T)) &&
                !t.IsAbstract &&
                !t.IsInterface);
        }
    }
}
