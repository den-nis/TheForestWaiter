using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Essentials
{
    public static class TrigHelper
    {
        public static float Up => -(float)Math.PI / 2;
        public static float Down => (float)Math.PI / 2;
        public static float Right => (float)Math.PI * 2;

        public static Vector2f FromAngleDeg(float degrees, float distance) => FromAngleRad(ToRad(degrees), distance);
        
        public static Vector2f FromAngleRad(float radians, float distance)
        {
            return new Vector2f(
                    (float)Math.Cos(radians) * distance,
                    (float)Math.Sin(radians) * distance
                );
        }

        public static float ToRad(float degrees) => degrees * (float)Math.PI / 180f;

        public static float ToDeg(float radians) => radians * 180f / (float)Math.PI;
    }
}
