using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Extensions
{
    public static class Vector2fExtensions
    {
        public static float Angle(this Vector2f vec)
        {
            return (float)Math.Atan2(vec.Y, vec.X);
        }

        public static Vector2f RotateBy(this Vector2f vec, float radians)
        {
            return new Vector2f
            (
                vec.X * (float)Math.Cos(radians) - vec.Y * (float)Math.Sin(radians),
                vec.X * (float)Math.Sin(radians) + vec.Y * (float)Math.Cos(radians)
            );
        }

        public static Vector2f Abs(this Vector2f vec)
        {
            return new Vector2f(Math.Abs(vec.X), Math.Abs(vec.Y));
        }

        public static float Len(this Vector2f vec)
        {
            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }
    }
}
