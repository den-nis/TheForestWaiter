﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Weapons.Bullets
{
    class Bullet : PhysicsObject
    {
        public float Range { get; set; }

        protected int Damage { get; set; } = 5;

        protected Sprite BulletSprite { get; set; }
        private bool HasHit { get; set; } = false;

        private Vector2f? _spawn = null;
        private float? _startAngle = null;
        private readonly ContentSource _content;

        private float Traveled { get; set; } = 0;


        public Bullet(GameData game, ContentSource content) : this(game, content, "Textures\\Bullets\\bullet_generic.png")
        {
            
        }

        public Bullet(GameData game, ContentSource content, string texture) : base(game)
        {
            SetBulletSprite(content.Textures.CreateSprite(texture));

            CollisionRadius = 10;
            Gravity = 0;
            Size = new Vector2f(5, 5);
            RespondToWorldCollision = false;
            _content = content;
        }

        protected void SetBulletSprite(Sprite bulletSprite)
        {
            BulletSprite = bulletSprite;
            BulletSprite.Origin = bulletSprite.Texture.Size.ToVector2f() / 2;
        }

        private void Explode()
        {
            MarkedForDeletion = true;
            HasHit = true;
            Velocity = default;

            var prop = _content.Particles.Get("Particles\\spark.particle", Center);
            prop.Life = 0.2f;

            Game.Objects.WorldParticles.Emit(prop, 10);
        }

        public override void Update(float time)
        {
            PhysicsTick(time);

            _spawn ??= Center;
            _startAngle ??= Velocity.Angle();

            foreach (var enemy in Game.Objects.Actors)
            {
                if (Collisions.SweptAABB(enemy.FloatRect, FloatRect, Velocity * time, out _) < 1 || Intersects(enemy))
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

            BulletSprite.Rotation = TrigHelper.ToDeg(_startAngle.Value);
            BulletSprite.Position = Center;

            if (TouchingHorizontal || TouchingVertical)
            {
                Explode();
            }
        }

		public override void Draw(RenderWindow window)
        {
            if (!HasHit)
                window.Draw(BulletSprite);
        }
    }
}
