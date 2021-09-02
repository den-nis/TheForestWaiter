using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheForestWaiter
{
    internal class DisablePropertyDependencies : IPropertyDependencySelector
    {
        public IEnumerable<PropertyDependency> Execute(Type type)
        {
            return Enumerable.Empty<PropertyDependency>();
        }
    }
}