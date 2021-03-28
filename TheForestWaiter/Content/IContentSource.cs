using System.IO;

namespace TheForestWaiter.Content
{
    interface IContentSource
    {
        Stream GetStream(string name);
        byte[] GetBytes(string name);
        string GetString(string name);
    }
}
