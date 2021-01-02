using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Resources
{
    public enum ResourceScope
    {
        Global,
        Game,
        UI,
    }

    class ResourceInfo
    {
        public ResourceInfo(object data, ResourceScope scope)
        {
            Data = data;
            Scope = scope;
        }

        public object Data { get; set; }
        public ResourceScope Scope { get; set; }
    }
}
