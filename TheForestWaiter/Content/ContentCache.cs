using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Config;

namespace TheForestWaiter.Content
{
    abstract class ContentCache<T> where T : class
    {
        protected abstract ContentType Type { get; }

        private Dictionary<string, T> Content { get; set; } = new Dictionary<string, T>();
        protected ContentConfig Config { get; private set; }

        public ContentCache(ContentConfig config)
        {
            Config = config;
        }

        public virtual T Get(string name)
        {
            Debug.Assert(Content.ContainsKey(name), $"Cannot find content \"{name}\"");
            return Content[name];
        }

        public void LoadFromSource(IContentSource source)
        {
            foreach(var meta in Config.GetFilesOfType(Type))
            {
                var content = LoadFromBytes(source.GetBytes(meta));
                Content.Add(meta, content);
            }
        }

        protected abstract T LoadFromBytes(byte[] bytes);
    }
}
