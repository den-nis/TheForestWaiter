using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Particles;

namespace TheForestWaiter.Game.Objects.Enemies
{
    class Rusher : Creature
    {
        private const float PARTICLES_PER_SECOND = 12;
        private const float ATTACK_DAMAGE = 9001;

        private readonly ParticleSystem _particles = new(500);
        private float _particleTimer = 0;

        private readonly Sprite _sprite;
        private readonly ContentSource _content;
        private float _speed = 200;

        public Rusher(GameData game, ContentSource content) : base(game)
        {
            _content = content;
            _sprite = content.Textures.CreateSprite("Textures\\Enemies\\rusher.png");
            Size = _sprite.Texture.Size.ToVector2f();

            EnableWorldCollisions = false;
            RespondToWorldCollision = true;
            EmitPhysicsCollisions = true;
            ReceivePhysicsCollisions = false;

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

            PhysicsTick(time);

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
                var prop = _content.Particles.Get("Particles\\rusher_smoke.particle", Center, TrigHelper.Right, _speed);
                _particles.Emit(prop);
            }

            _particles.Update(time);
        }

        public void HandleAnimations()
        {
            _sprite.Position = Position;
        }

        protected override void OnTouch(PhysicsObject obj)
        {
            if (obj is Player p)
                p.Damage(this, ATTACK_DAMAGE);
        }

		protected override void OnDeath()
		{
		}

		protected override void OnDamage(PhysicsObject by)
		{
		}
	}
}
