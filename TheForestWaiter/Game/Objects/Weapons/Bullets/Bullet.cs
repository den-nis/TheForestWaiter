using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Weapons.Bullets
{
    class Bullet : DynamicObject
    {
        public float Range { get; set; }

        protected int Damage { get; set; } = 5;

        protected Sprite BulletSprite { get; set; }
        protected float StartAngle { get; }
        private bool HasHit { get; set; } = false;

        private Vector2f? _spawn = null;
        private float? _startAngle = null;
        private readonly GameContent _content;

        private float Traveled { get; set; } = 0;

        public Bullet(GameData game, IGameDebug debug, GameContent content) : base(game, debug)
        {
            CollisionRadius = 10;

            Gravity = 0;
            Size = new Vector2f(5, 5);
            StartAngle = velocity.Angle();
            RespondToWorldCollision = false;

            BulletSprite = content.Textures.CreateSprite("Textures\\Bullets\\bullet_generic.png");
            BulletSprite.Origin = Size / 2;
            BulletSprite.Rotation = TrigHelper.ToDeg(StartAngle);
            _content = content;
        }

        private void Explode()
        {
            MarkedForDeletion = true;
            HasHit = true;
            velocity = default;

            var prop = _content.Particles.Get("Particles\\spark.particle", Center);
            prop.Life = 0.2f;

            Game.Objects.WorldParticles.Emit(prop, 10);
        }

        public override void Update(float time)
        {
            _spawn ??= Center;
            _startAngle ??= velocity.Angle();

            foreach (var enemy in Game.Objects.Enemies)
            {
                if (Collisions.SweptAABB(enemy.FloatRect, FloatRect, velocity * time, out _) < 1)
                {
                    enemy.Damage(this, Damage);
                    Explode();
                }
            }

            Traveled = (Center - _spawn.Value).Len();
            if (Traveled > Range)
            {
                Explode();
                return;
            }

            BulletSprite.Position = Center;

            if (TouchingHorizontal || TouchingVertical)
            {
                Explode();
            } 

            base.Update(time);
        }

		public override void Draw(RenderWindow window)
        {
            if (!HasHit)
                window.Draw(BulletSprite);
        }
    }
}
