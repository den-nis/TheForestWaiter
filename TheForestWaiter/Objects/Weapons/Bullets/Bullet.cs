using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Entites;
using TheForestWaiter.Essentials;
using TheForestWaiter.Particles;
using TheForestWaiter.Particles.Templates;

namespace TheForestWaiter.Objects.Weapons.Bullets
{
    class Bullet : DynamicObject
    {
        private Sprite BulletSprite { get; set; }
        private float StartAngle { get; }
        private bool HasHit { get; set; } = false;
        
        private Vector2f Spawn { get; }
        private float Range { get; }
        private float Traveled { get; set; } = 0;

        public Bullet(GameData game, Vector2f spawn, Vector2f speed, float range) : base(game)
        {
            Range = range;
            Gravity = 0;
            Size = new Vector2f(5, 5);
            Center = spawn;
            Spawn = spawn;
            velocity = speed;
            StartAngle = velocity.Angle();
            RespondToCollision = false;

            BulletSprite = new Sprite(GameContent.GetTexture("Content.Textures.Bullets.bullet_generic.png"))
            {
                Origin = Size / 2,
                Rotation = TrigHelper.ToDeg(StartAngle)
            };
        }

        private void Explode()
        {
            MarkedForDeletion = true;
            HasHit = true;
            velocity = default;

            var prop = ParticleTemplates.Spark;
            prop.Position = Center;
            prop.Life = 0.2f;

            Game.Objects.WorldParticles.Emit(prop, 10);
        }

        public override void Update(float time)
        {
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
