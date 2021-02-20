using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Essentials;

namespace TheForestWaiter.Particles
{
    // Inspired by Cherno https://github.com/TheCherno/OneHourParticleSystem
    class ParticleSystem
    {
        private static RectangleShape ParticleShape { get; set; } = new RectangleShape();
        public bool IsActive { get; private set; }

        private struct Particle
        {
            public Color ColorStart { get; set; }
            public Color ColorEnd { get; set; }

            public Vector2f Velocity { get; set; }

            public Vector2f Position { get; set; }

            public float SizeStart { get; set; }
            public float SizeEnd { get; set; }

            public float RotationSpeed { get; set; }

            public float Rotation { get; set; }
            public float RemainingLife { get; set; }

            public float Life { get; set; }
        }

        private Particle[] ParticlePool { get; set; }

        public int Index => PoolIndex;
        private int PoolIndex { get; set; } = 0;

        public ParticleSystem(int poolSize)
        {
            ParticlePool = new Particle[poolSize];
        }

        private static float Lerp(float start, float end, float value) => start + ((end - start) * value);

        private static byte Lerp(byte start, byte end, float value) => (byte)(start + ((end - start) * value));

        private static Vector2f Lerp(Vector2f start, Vector2f end, float value)
        {
            return new Vector2f
            {
                X = Lerp(start.X, end.X, value),
                Y = Lerp(start.Y, end.Y, value)
            };
        }

        private static Color Lerp(Color start, Color end, float value)
        {
            return new Color
            {
                R = Lerp(start.R, end.R, value),
                G = Lerp(start.G, end.G, value),
                B = Lerp(start.B, end.B, value),
                A = Lerp(start.A, end.A, value)
            };
        }

        public void Draw(RenderWindow window)
        {
            foreach(var p in ParticlePool)
            {
                if (p.RemainingLife > 0)
                {
                    var life = 1 - (p.RemainingLife / p.Life);

                    ParticleShape.FillColor = Lerp(p.ColorStart, p.ColorEnd, life);
                    ParticleShape.Size = Lerp(new Vector2f(p.SizeStart, p.SizeStart), new Vector2f(p.SizeEnd, p.SizeEnd), life);
                    ParticleShape.Origin = ParticleShape.Size / 2;
                    ParticleShape.Rotation = TrigHelper.ToDeg(p.Rotation);
                    ParticleShape.Position = p.Position;
                    window.Draw(ParticleShape);
                }
            }
        }

        public void Update(float time)
        {
            IsActive = false;
            for (int i = 0; i < ParticlePool.Length; i++)
            {
                ref Particle p = ref ParticlePool[i];
                if (p.RemainingLife > 0)
                {
                    IsActive = true;
                    p.Position += p.Velocity * time;
                    p.Rotation += p.RotationSpeed * time;
                    p.RemainingLife -= time;
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < ParticlePool.Length; i++)
            {
                ref Particle p = ref ParticlePool[i];
                p.RemainingLife = 0;
            }
        }

        public void Emit(ParticleProp prop, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Emit(prop);
            }
        }

        public void Emit(ParticleProp prop)
        {
            if (GameSettings.Current.DisabledParticles)
                return;

            ref Particle p = ref ParticlePool[PoolIndex];

            p.ColorStart = prop.ColorStart;
            p.ColorEnd = prop.ColorEnd;
            p.Position = prop.Position;

            p.Velocity = new Vector2f(
                    prop.Velocity.X + (prop.VelocityVariation.X * (Rng.Float() - 0.5f)),
                    prop.Velocity.Y + (prop.VelocityVariation.Y * (Rng.Float() - 0.5f))
                );

            p.SizeStart = prop.SizeStart + (prop.SizeVariation * (Rng.Float() - 0.5f));
            p.SizeEnd   = prop.SizeEnd   + (prop.SizeVariation * (Rng.Float() - 0.5f));

            p.Life = prop.Life + (prop.LifeVariation * (Rng.Float() - 0.5f));
            p.RemainingLife = p.Life;
            p.RotationSpeed = prop.RotationSpeed + (prop.RotationSpeedVariation * (Rng.Float() - 0.5f));

            PoolIndex = (PoolIndex + 1) % ParticlePool.Length;
        }
    }
}
