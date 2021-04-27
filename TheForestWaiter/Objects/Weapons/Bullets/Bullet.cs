using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Entities;
using TheForestWaiter.Essentials;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Objects.Weapons.Bullets
{
    class Bullet : DynamicObject
    {
        protected int Damage { get; set; } = 5;

        protected Sprite BulletSprite { get; set; }
        protected float StartAngle { get; }
        private bool HasHit { get; set; } = false;
        
        private Vector2f Spawn { get; }
        private float Range { get; }
        private float Traveled { get; set; } = 0;

        public Bullet(GameData game, Vector2f spawn, Vector2f speed, float range) : base(game)
        {
            CollisionRadius = 10;

            Range = range;
            Gravity = 0;
            Size = new Vector2f(5, 5);
            Center = spawn;
            Spawn = spawn;
            velocity = speed;
            StartAngle = velocity.Angle();
            RespondToCollision = false;

            BulletSprite = GameContent.Textures.CreateSprite("Textures\\Bullets\\bullet_generic.png");
            BulletSprite.Origin = Size / 2;
            BulletSprite.Rotation = TrigHelper.ToDeg(StartAngle);
        }

        private void Explode()
        {
            MarkedForDeletion = true;
            HasHit = true;
            velocity = default;

            var prop = GameContent.Particles.Get("Particles\\spark.particle", Center);
            prop.Life = 0.2f;

            Game.Objects.WorldParticles.Emit(prop, 10);
        }

        public override void Update(float time)
        {
            foreach(var enemy in Game.Objects.Enemies)
            {
                if (Collisions.SweptAABB(enemy.FloatRect, FloatRect, velocity * time, out _) < 1)
                {
                    enemy.Damage(this, Damage);
                    Explode();
                }
            }

            Traveled = (Center - Spawn).Len();
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
