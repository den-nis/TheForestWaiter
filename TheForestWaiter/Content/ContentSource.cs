using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    class ContentSource
    {
        public IContentSource Source { get; private set; }
        public ContentConfig Config { get; private set; }

        public TextureCache Textures { get; private set; }
        public FontCache Fonts { get; private set; }
        public ParticleCache Particles { get; private set; }

        public void Setup()
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

            Particles = new ParticleCache(Config);
            Particles.LoadFromSource(Source);
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in release mode")]
        private IContentSource GetContentSource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(ContentSettings.CONTENT_EMBEDDED_FILE);
            return GetContentSourceFromStream(stream);
        }

        private IContentSource GetDebugContentSource()
        {
            var stream = File.OpenRead(ContentSettings.CONTENT_FILE);
            return GetContentSourceFromStream(stream);
        }

        private IContentSource GetContentSourceFromStream(Stream stream)
        {
            ZipArchive zip = new(stream);
            return new ZipContentSource(zip);
        }
    }
}
