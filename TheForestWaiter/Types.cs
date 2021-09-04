using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Entities;

namespace TheForestWaiter
{
    public static class Types
    {
        public static IEnumerable<Type> GameObjects { get; private set; }

        static Types()
        {
            GameObjects = GetGameObjects();
        }

        private static IEnumerable<Type> GetGameObjects()
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(t => 
                t.IsAssignableTo(typeof(GameObject)) &&
                !t.IsAbstract &&
                !t.IsInterface);
        }
    }
}
