using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Content
{
    class FontCache : ContentCache<Font>
    {
        protected override ContentType Type => ContentType.Font;

        public FontCache(ContentConfig config) : base(config)
        {

        }

        protected override Font LoadFromBytes(byte[] bytes)
        {
            return new Font(bytes);
        }
    }
}
