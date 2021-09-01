using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Variables
{
    [AttributeUsage(AttributeTargets.Method)]
    class VariableAttribute : Attribute
    {
        public string Name { get; }

        public VariableAttribute(string name)
        {
            Name = name;
        }
    }
}
