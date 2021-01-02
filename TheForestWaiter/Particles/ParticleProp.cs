using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Particles
{
    class ParticleProp
    {
        public Color ColorStart { get; set; }
        public Color ColorEnd { get; set; }

        public Vector2f VelocityVariation { get; set; }

        public Vector2f Velocity { get; set; }
        public Vector2f Position { get; set; }

        public float SizeVariation { get; set; }
        public float SizeStart { get; set; }
        public float SizeEnd { get; set; }

        public float RotationSpeed { get; set; }
        public float RotationSpeedVariation { get; set; }

        public float LifeVariation { get; set; }
        public float Life { get; set; } = 1;
    }
}
