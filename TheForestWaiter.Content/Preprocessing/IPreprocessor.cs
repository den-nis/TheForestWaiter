using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    interface IPreprocessor
    {
        byte[] Process(byte[] input, ContentMeta meta);
    }
}
