using SFML.Graphics;
using SFML.System;
using System;

namespace TheForestWaiter.Game.Particles
{
	class ParticleProp : ICloneable
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

		public object Clone()
		{
			return new ParticleProp
			{
				ColorStart = ColorStart,
				ColorEnd = ColorEnd,
				VelocityVariation = VelocityVariation,
				Velocity = Velocity,
				Position = Position,
				SizeVariation = SizeVariation,
				SizeStart = SizeStart,
				SizeEnd = SizeEnd,
				RotationSpeed = RotationSpeed,
				RotationSpeedVariation = RotationSpeedVariation,
				LifeVariation = LifeVariation,
				Life = Life,
			};
		}
	}
}
