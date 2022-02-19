﻿using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    class TexturePreprocessor : IPreprocessor
    {
        public TexturePreprocessor()
        {

        }

        public byte[] Process(byte[] input, ContentMeta meta)
        {
            if (meta.TextureConfig.HasTileSpacing)
            {
                Console.WriteLine("Resizing texture...");
                SpacedImageBuilder builder = new(meta.TextureConfig.TileWidth, meta.TextureConfig.TileHeight);
                var result = builder.BuildSpacedImage(new MagickImage(input));
                return result.ToByteArray(MagickFormat.Png);
            }

            return input;
        }
    }
}
