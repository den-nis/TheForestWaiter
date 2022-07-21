using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class CorruptionBall : SmallBullet
	{
		private const float PARTICLE_SPAWN_INTERVAL = 0.01f;
		private float _particleTimer = -1;

		private readonly ContentSource _content;

		public CorruptionBall()
		{
			_content = IoC.GetInstance<ContentSource>();

			ExplosionParticleName = "Particles/corruption_ball_smoke.particle";
			Damage = 40;

			SetTexture("Textures/Projectiles/corruption_ball.png");
		}

		public override void Update(float time)
		{
			base.Update(time);

			if (_particleTimer <= 0)
			{
				var prop = _content.Particles.Get(ExplosionParticleName, Center);
				Game.Objects.WorldParticles.Emit(prop, 1);
				_particleTimer = PARTICLE_SPAWN_INTERVAL;
			}
			else
			{
				_particleTimer -= time;
			}
		}
	}
}
