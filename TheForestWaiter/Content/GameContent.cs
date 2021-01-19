using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Config;
using TheForestWaiter.Content;

namespace TheForestWaiter
{
    static class GameContent
    {
        public static IContentSource Source { get; private set; }
        public static ContentConfig Config { get; private set; }

        public static TextureCache Textures { get; private set; }

        public static void Initialize()
        {
#if DEBUG
            Source = GetDebugContentSource();
#else
            Source = GetContentSource();
#endif

            var configJson = Source.GetString(ContentSettings.CONTENT_CONFIG_ENTRY);
            Config = JsonConvert.DeserializeObject<ContentConfig>(configJson);

            //Load textures
            Textures = new TextureCache(Config);
            Textures.LoadFromSource(Source);
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static IContentSource GetContentSource()
#pragma warning restore IDE0051
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(ContentSettings.CONTENT_EMBEDDED_FILE);
            return GetContentSourceFromStream(stream);
        }

        private static IContentSource GetDebugContentSource()
        {
            var stream = File.OpenRead(ContentSettings.CONTENT_FILE);
            return GetContentSourceFromStream(stream);
        }

        private static IContentSource GetContentSourceFromStream(Stream stream)
        {
            ZipArchive zip = new ZipArchive(stream);
            return new ZipContentSource(zip);
        }
    }
}
