using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Environment
{
    class Chunks
    {
        public const float CHUNK_WIDTH = 100;
        public const float RENDER_DISTANCE = 2000;

        private World World { get; set; }
        public Chunks(World world)
        {

        }
    }
}
