using System.Collections.Generic;
using System.Diagnostics;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
	abstract class ContentCache<T> where T : class
	{
		protected abstract ContentType Type { get; }

		private readonly Dictionary<string, T> _content = new();
		protected ContentConfig Config { get; private set; }

		public ContentCache(ContentConfig config)
		{
			Config = config;
		}

		public virtual T Get(string name)
		{
			Debug.Assert(_content.ContainsKey(name), $"Cannot find content \"{name}\"");
			return _content[name];
		}

		public void LoadFromSource(IContentSource source)
		{
			foreach (var meta in Config.GetFilesOfType(Type))
			{
				var content = LoadFromBytes(source.GetBytes(meta));
				_content.Add(meta, content);
			}
		}

		protected abstract T LoadFromBytes(byte[] bytes);
	}
}
