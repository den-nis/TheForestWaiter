using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Config;

namespace TheForestWaiter.Content
{
    class TexturePreprocessor : IPreprocessor
    {
        public TexturePreprocessor()
        {

        }

        public byte[] Process(byte[] input, ContentMeta meta)
        {
            if (meta.TextureHasTileSpacing)
            {
                Console.WriteLine("Resizing texture...");
                SpacedImageBuilder builder = new SpacedImageBuilder(meta.TextureTileWidth, meta.TextureTileHeight);
                var result = builder.BuildSpacedImage(new MagickImage(input));
                return result.ToByteArray(MagickFormat.Png);
            }

            return input;
        }
    }
}
