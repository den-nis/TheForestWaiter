using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Content
{
    interface IContentSource
    {
        Stream GetStream(string name);
        byte[] GetBytes(string name);
        string GetString(string name);
    }
}
