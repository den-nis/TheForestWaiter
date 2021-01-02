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
            Life = 1,
            RotationSpeedVariation = 10,
            SizeStart = 2,
            SizeEnd = 5,
            LifeVariation = 1,
            VelocityVariation = new Vector2f(20, 20)
        };

        public static ParticleProp Spark => new ParticleProp
        {
            ColorEnd = new Color(77,77,77,0),
            ColorStart = new Color(255, 238, 77),
            Life = 1,
            RotationSpeedVariation = 10,
            SizeStart = 2,
            SizeEnd = 5,
            LifeVariation = 1,
            VelocityVariation = new Vector2f(20, 20)
        };
    }
}
