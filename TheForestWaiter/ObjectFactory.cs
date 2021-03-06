using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TheForestWaiter.Entites;
using TheForestWaiter.Objects.Static;

namespace TheForestWaiter
{
    class ObjectFactory
    {
        private bool _indexed = false;
        private readonly GameData _data;
        private Dictionary<string, Type> _staticObjects;

        public ObjectFactory(GameData data)
        {
            _data = data;
        }

        public void Index()
		{
            var asm = Assembly.GetExecutingAssembly();
            var staticObjects = asm.GetTypes().Where(t => t.IsAssignableTo(typeof(StaticObject)));

            _staticObjects = staticObjects.ToDictionary(k => k.Name);
            _indexed = true;
		}

        public StaticObject GetStaticObject(string name)
        {
            if (!_indexed)
                throw new InvalidOperationException("ObjectFactory must first be indexed");


            if (_staticObjects.TryGetValue(name, out Type type))
            {
                return (StaticObject)Activator.CreateInstance(type, new object[] { _data });
            }
            else
			{
                throw new KeyNotFoundException($"No such StaticObject named \"{name}\"");
			}
        }
    }
}
