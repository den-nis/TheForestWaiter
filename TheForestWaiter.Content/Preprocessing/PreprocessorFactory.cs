using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    static class PreprocessorFactory
    {
        public static IPreprocessor GetPreprocessor(ContentType contentType)
        {
            return contentType switch
            {
                ContentType.Texture => new TexturePreprocessor(),
                _ => null,
            };
        }
    }
}
