using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO.Compression;

namespace TheForestWaiter.Shared
{
    public enum ContentType
    {
        Raw = 0,
        Texture,
        Font,
        Particle,
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

        public Dictionary<string, ContentMeta> Content { get; set; }

        public IEnumerable<string> GetFilesOfType(ContentType type)
        {
            foreach(var i in Content)
            {
                if (i.Value.Type == type)
                    yield return i.Key;
            }
        }
    }

    public class ContentMeta
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public CompressionLevel Compression { get; set; }

        public int  TextureTileWidth      { get; set; }
        public int  TextureTileHeight     { get; set; }
        public int  TextureFramerate      { get; set; }
        public bool TextureHasTileSpacing { get; set; }

        public bool SoundLooping { get; set; }
    }
}
