﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TheForestWaiter.Debugging
{
    class CommandInfo
    {
        public MethodInfo Method { get; set; }
        public CommandAttribute Attribute { get; set; }
    }
}
