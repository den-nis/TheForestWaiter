﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Config;

namespace TheForestWaiter.Content
{
    public static class ContentTypeDefinitions
    {
        public static ContentType FromFilename(string filename)
        {
            var ext = Path.GetExtension(filename);
            return ext switch
            {
                ".png" or ".jpg" => ContentType.Texture,
                ".ttf" => ContentType.Font,
                _ => ContentType.Raw,
            };
        }
    }
}