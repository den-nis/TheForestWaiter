using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Entities;
using TheForestWaiter.Essentials;
using TheForestWaiter.Gibs;
using TheForestWaiter.Graphics;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Objects.Enemies
{
    class Rusher : Creature
    {
        private const float PARTICLES_PER_SECOND = 12;
        private const float ATTACK_DAMAGE = 9001;

        private readonly ParticleSystem _particles = new(500);
        private float _particleTimer = 0;

        private readonly Sprite _sprite;
        private float _speed = 200;

        public Rusher(GameData game) : base(game)
        {
            _sprite = GameContent.Textures.CreateSprite("Textures\\Enemies\\rusher.png");
            Size = _sprite.Texture.Size.ToVector2f();

            EnableWorldCollisions = false;
            RespondToCollision = true;
            EmitDynamicCollisions = true;
            ReceiveDynamicCollisions = false;

            Gravity = 0;
        }

        public override void Draw(RenderWindow window)
        {
            _particles.Draw(window);
            window.Draw(_sprite);
        }

        public override void Update(float time)
        {
            _speed += time * 3;

            if (Game.Objects.Player.Center.X < Center.X)
                Game.Objects.Player.Damage(this, ATTACK_DAMAGE);

            var direction = (Game.Objects.Player.Center - Center).Norm();
            LimitPush(new Vector2f(1, direction.Y) * _speed, time * 5);

            HandleParticles(time);
            HandleAnimations();
            base.Update(time);
        }

        private void HandleParticles(float time)
        {
            _particleTimer += time;

            //TODO: timer helper?
            while (_particleTimer > (1 / PARTICLES_PER_SECOND))
            {
                _particleTimer -= 1 / PARTICLES_PER_SECOND;
                var prop = GameContent.Particles.Get("Particles\\rusher_smoke.particle", Center, TrigHelper.Right, _speed);
                _particles.Emit(prop);
            }

            _particles.Update(time);
        }

        public void HandleAnimations()
        {
            _sprite.Position = Position;
        }

        protected override void OnTouch(DynamicObject obj)
        {
            if (obj is Player p)
                p.Damage(this, ATTACK_DAMAGE);
        }

		protected override void OnDeath()
		{
		}

		protected override void OnDamage(DynamicObject by)
		{
		}
	}
}
