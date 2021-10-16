
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Gibs;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Objects.Enemies
{
    class Crawler : Enemy
    {
        private const float ATTACK_STRIKE_TIME = .5f;
        private const float ATTACK_TOTAL_TIME = .7f;
        private const float ATTACK_DAMAGE = 50;
        private readonly AnimatedSprite _animation;
        private readonly ContentSource _content;
        private readonly GibSpawner _gibSpawner;
        private int _facingDirection = 0;
        private int _jumpVelocity;

        private Vector2f AttackPoint => Center + new Vector2f(_facingDirection * 20, 0);
        private Vector2f ParticlePoint => Center + new Vector2f(_facingDirection * 15, 0);
        private bool _hasStriked;
        private bool _isAttacking;
        private float _attackTimer;

        public Crawler(GameData game, ContentSource content, GibSpawner gibSpawner) : base(game)
        {
            _animation = content.Textures.CreateAnimatedSprite("Textures\\Enemies\\crawler.png");
            Size = _animation.Sheet.TileSize.ToVector2f();
            
            Speed = 200;
            _jumpVelocity = 400;
            AngerRadius = 0;

            _content = content;
            _gibSpawner = gibSpawner;
            _gibSpawner.Sheet = content.Textures.CreateSpriteSheet("Textures\\Enemies\\minirusher_gibs.png");

            CollisionRadius = 42;
            Health = 10;
        }

        public void HandleAnimations(float time)
        {
            _animation.Sprite.Color = IsStunned ? new Color(255, 0, 0) : Color.White;
            _animation.Sprite.Position = Position;
            _animation.Sheet.MirrorX = _facingDirection == 1;


            if (_isAttacking)
            {
                _animation.Framerate = 10;
                _animation.AnimationStart = 7;
                _animation.AnimationEnd = 12;
                _animation.Paused = false;
            }
            else
            {
                var isMovingRight = RealSpeed.X > 1;
                var isMovingLeft = RealSpeed.X < -1;

                if (isMovingLeft || isMovingRight)
                {
                    _animation.Framerate = 20;
                    _animation.AnimationStart = 14;
                    _animation.AnimationEnd = 18;
                    _animation.Paused = false;
                }
                else
                {
                    _animation.Framerate = 7;
                    _animation.AnimationStart = 0;
                    _animation.AnimationEnd = 6;
                    _animation.Paused = false;
                }
            }

            _animation.Update(time);
        }

        public override void Draw(RenderWindow window)
        {
            window.Draw(_animation);
        }

        public override void Update(float time)
        {
            if (_isAttacking)
            {
                SetVelocityX(0);
                _attackTimer += time;
                if (!_hasStriked && _attackTimer > ATTACK_STRIKE_TIME)
                {
                    _hasStriked = true;
                    Attack();
				}

                if (_attackTimer > ATTACK_TOTAL_TIME)
                {
                    _attackTimer = 0;
                    _hasStriked = false;
                    _isAttacking = false;
				}
			}

            base.Update(time);
            PhysicsTick(time);
            HandleAnimations(time);
        }

        private void Jump()
        {
            if (TouchingFloor)
                SetVelocityY(-_jumpVelocity);
        }

        public override void OnAngry(float time)
        {
            if (!_isAttacking)
            {
                EnableDrag = false;
                var moved = AdvanceToPlayerOnGround(time, Jump);
                _facingDirection = moved;
            }
	    }

		protected override void OnTouch(PhysicsObject obj)
        {
            if (obj is Player && _facingDirection != 0 && !_isAttacking)
            {
                _isAttacking = true;
            }

            base.OnTouch(obj);
        }

        private void Attack()
        {
            var rect = new FloatRect(AttackPoint, new Vector2f(1, 1));
            var smoke = _content.Particles.Get("Particles\\crawler_strike.particle", ParticlePoint);
            Game.Objects.WorldParticles.Emit(smoke, 15);
            if (Game.Objects.Player.FloatRect.Intersects(rect))
            {
                Game.Objects.Player.Damage(this, ATTACK_DAMAGE);
			}
        }

        public override void OnIdle(float time) 
        {
            EnableDrag = true;
        }

        protected override void OnDeath() 
        {
            var prop = _content.Particles.Get("Particles\\blood.particle", Center);
            Game.Objects.WorldParticles.Emit(prop, 12);

            _gibSpawner.InitialVelocity = Velocity;
            _gibSpawner.SpawnAll(Center);
            MarkedForDeletion = true;
        }

        protected override void OnDamage(PhysicsObject by)
        {
            var prop = _content.Particles.Get("Particles\\blood.particle", Center);
            Game.Objects.WorldParticles.Emit(prop, 5);

            ApplyStunMovement(by);
            base.OnDamage(by);
        }
    }
}
