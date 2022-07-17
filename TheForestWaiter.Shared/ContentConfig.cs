using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace TheForestWaiter.Shared
{
    public enum ContentType
    {
        Raw = 0,
        Texture,
        Font,
        Particle,
        Sound,
    }

    public class ContentConfig
    {
        public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
#if DEBUG 
            Formatting = Formatting.Indented,
#endif
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        };

        public List<ContentMeta> Content { get; set; }

        public IEnumerable<string> GetFilesOfType(ContentType type)
        {
            foreach (var i in Content)
            {
                if (i.Type == type)
                    yield return i.Path;
            }
        }

        public bool HasFile(string path)
        {
            return Content.Any(c => c.Path == path);
        }

        public ContentMeta TryGetByPath(string path)
        {
            return Content.FirstOrDefault(c => c.Path == path);
        }

        public ContentMeta GetByPath(string path)
        {
            return Content.First(c => c.Path == path);
        }
    }
    
    public class ContentMeta
    {
        public string Path { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public CompressionLevel Compression { get; set; }

        public TextureConfig TextureConfig { get; set; } = new TextureConfig();
    }

    public class TextureConfig
    {
        public int TileWidth { get; set; }
        public int Framerate { get; set; }
        public int TileHeight { get; set; }
        public bool HasTileSpacing { get; set; }
        public List<SheetSection> Sections { get; set; }
    }

    public class SheetSection
    {
        public string Name { get; set; }
        public int Fps { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int? FixedFrame { get; set; }
    }
}
