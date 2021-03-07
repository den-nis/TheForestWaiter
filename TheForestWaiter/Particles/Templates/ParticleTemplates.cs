using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Particles.Templates
{
    static class ParticleTemplates
    {
        public static ParticleProp GenericSmoke => new ParticleProp
        {
            ColorEnd = Color.Black,
            ColorStart = new Color(100, 100, 100),
            Life = 2,
            RotationSpeedVariation = 10,
            SizeStart = 2,
            SizeEnd = 5,
            LifeVariation = 1,
            VelocityVariation = new Vector2f(20, 20)
        };

        public static ParticleProp SniperSmoke => new ParticleProp
        {
            ColorEnd = new Color(255,255,255,0),
            ColorStart = new Color(100, 100, 100),
            Life = 2,
            RotationSpeedVariation = 10,
            SizeStart = 2,
            SizeEnd = 2,
            LifeVariation = 1,
            Velocity = new Vector2f(0, -10),
            VelocityVariation = new Vector2f(3, 12)
        };


        public static ParticleProp Spark => new ParticleProp
        {
            ColorStart = new Color(255, 220, 198),
            ColorEnd = new Color(255,178,0,0),
            Life = 1,
            RotationSpeedVariation = 10,
            SizeStart = 2,
            SizeEnd = 5,
            LifeVariation = 1,
            VelocityVariation = new Vector2f(150, 150)
        };

        public static ParticleProp Blood => new ParticleProp
        {
            ColorEnd = Color.Transparent,
            ColorStart = new Color(150,0,0),
            Life = 1,
            RotationSpeedVariation = 10,
            SizeStart = 2,
            SizeEnd = 5,
            LifeVariation = 1,
            VelocityVariation = new Vector2f(10, 10)
        };
    }
}
