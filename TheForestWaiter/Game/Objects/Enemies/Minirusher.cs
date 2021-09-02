﻿
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
    class Minirusher : Enemy
    {
        private const float ATTACK_DAMAGE = 10;
        private readonly AnimatedSprite _animation;
        private readonly RandomTrigger _jumpTrigger;
        private readonly GameContent _content;
        private readonly GibSpawner _gibSpawner;
        private int _targetDirection = 0;

        public Minirusher(GameData game, GameContent content, GibSpawner gibSpawner, IGameDebug debug) : base(game, debug)
        {
            _animation = content.Textures.CreateAnimatedSprite("Textures\\Enemies\\minirusher.png");
            Size = _animation.Sheet.TileSize.ToVector2f();
            
            StunTime = 0.1f;
            Speed = 300;
            Drag = new Vector2f(253, 0);
            JumpVelocity = 450;
            TriggeredRadius = 900;

            _jumpTrigger = new RandomTrigger(Jump, 70, 1);
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

            var isMovingRight = RealSpeed.X > 50;
            var isMovingLeft = RealSpeed.X < -50;

            if (_targetDirection == 0)
            {
                if (isMovingLeft)
                    _animation.Sheet.MirrorX = false;

                if (isMovingRight)
                    _animation.Sheet.MirrorX = true;
			}
            else
			{
                _animation.Sheet.MirrorX = _targetDirection == 1;
            }

            if (!isMovingLeft && !isMovingRight && TouchingFloor)
            {
                _animation.SetStaticFrame(0);
                _animation.Paused = true;
            }
            else
            {
                _animation.AnimationStart = 0;
                _animation.AnimationEnd = 7;
                _animation.Paused = false;
            }
            _animation.Update(time);
        }

        public override void Draw(RenderWindow window)
        {
            window.Draw(_animation);
        }

        public override void Update(float time)
        {
            HandleAnimations(time);
            base.Update(time);
        }

        private void Jump()
        {
            if (TouchingFloor)
                velocity.Y = -JumpVelocity;
        }

        public override void OnAngry(float time)
        {
            _jumpTrigger.TryTrigger(time);

            if (Math.Abs(RealSpeed.X) < 10)
                Jump();

            var direction = (Game.Objects.Player.Center - Center).X;
            direction = Math.Max(-1, Math.Min(1, direction));
            _targetDirection = Math.Sign(direction);

            LimitPush(new Vector2f(direction, 0) * 500, time);
        }

        protected override void OnTouch(DynamicObject obj)
        {
            if (obj is Player p)
                p.Damage(this, ATTACK_DAMAGE);
        }

        public override void OnIdle(float time) 
        {
            _targetDirection = 0;
        }

        protected override void OnDeath() 
        {
            var prop = _content.Particles.Get("Particles\\blood.particle", Center);
            Game.Objects.WorldParticles.Emit(prop, 12);

            _gibSpawner.SpawnComplete(Center);
            MarkedForDeletion = true;
        }

        protected override void OnDamage(DynamicObject by)
        {
            var prop = _content.Particles.Get("Particles\\blood.particle", Center);
            Game.Objects.WorldParticles.Emit(prop, 5);

            ApplyKnockback(by);
            base.OnDamage(by);
        }
    }
}