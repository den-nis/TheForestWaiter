using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Entites;
using TheForestWaiter.Objects.Static;

namespace TheForestWaiter
{
    class ObjectFactory
    {
        private readonly GameData _data;

        public ObjectFactory(GameData data)
        {
            _data = data;
        }

        public StaticObject GetStaticObject(string name)
        {
            switch(name)
            {
                case "Tree": return new Tree(_data);

                default:
                    throw new ArgumentException($"Invalid static object \"{name}\"");
            }
        }
    }
}
