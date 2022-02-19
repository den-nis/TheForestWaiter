using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Objects.Projectiles
{
	internal class CorruptionBall : Bullet
	{
		private const float PARTICLE_SPAWN_INTERVAL = 0.01f;
		private float _particleTimer = -1;
		

		private readonly ContentSource _content;

		public CorruptionBall(GameData game, ContentSource content) : base(game, content)
		{
			_content = content;

			ExplosionParticleName = "Particles/corruption_ball_smoke.particle";
			Damage = 40;

			SetBulletSprite("Textures/Bullets/bullet_corruption_ball.png");
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
