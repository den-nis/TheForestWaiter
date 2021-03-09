using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using TheForestWaiter.Content;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    static class GameContent
    {
        public static IContentSource Source { get; private set; }
        public static ContentConfig Config { get; private set; }

        public static TextureCache Textures { get; private set; }
        public static FontCache Fonts { get; private set; }

        public static void Initialize()
        {
#if DEBUG
            Source = GetDebugContentSource();
#else
            Source = GetContentSource();
#endif

            var configJson = Source.GetString(ContentSettings.CONTENT_CONFIG_ENTRY);
            Config = JsonConvert.DeserializeObject<ContentConfig>(configJson);

            Textures = new TextureCache(Config);
            Textures.LoadFromSource(Source);

            Fonts = new FontCache(Config);
            Fonts.LoadFromSource(Source);
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
            ZipArchive zip = new(stream);
            return new ZipContentSource(zip);
        }
    }
}
