using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TheForestWaiter.Game.Entities;

namespace TheForestWaiter.Game
{
    //TODO: I think this class is no longer needed
    class ObjectFactory
    {
        private bool _indexed = false;
        private readonly GameData _data;
        private readonly ObjectCreator _creator;
        private Dictionary<string, Type> _staticObjects;

        public ObjectFactory(GameData data, ObjectCreator creator)
        {
            _data = data;
            _creator = creator;
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
                return (StaticObject)_creator.CreateType(type);
            }
            else
			{
                throw new KeyNotFoundException($"No such StaticObject named \"{name}\"");
			}
        }
    }
}
