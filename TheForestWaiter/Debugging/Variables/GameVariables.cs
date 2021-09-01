using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Variables
{
    /// <summary>
    /// Default values should be values for release
    /// </summary>
    class GameVariables
    {
        public float SimulateLag { get; set; } = 0;
        public float TimeScale { get; set; } = 1;
        public bool LockFramerate { get; set; } = false;
        public bool LagLimit { get; set; } = true;
        public bool LimitView { get; set; } = true;
    }
}
