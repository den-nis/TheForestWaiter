using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Game
{
    public static class Rng
    {
        private static Random Random { get; set; } = new Random();

        public static float Float()
        {
            return (float)Random.NextDouble();
        }

        public static float Range(float min, float max)
        {
            return min + Float() * (max - min);
        }
    }
}
