using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
    public static class Rng
    {
        private static Random Random { get; set; } = new Random();

        public static float Float()
        {
            return (float)Random.NextDouble();
        }
    }
}
