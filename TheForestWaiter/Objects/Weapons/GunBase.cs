using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Essentials;
using TheForestWaiter.Objects.Weapons.Bullets;

namespace TheForestWaiter.Objects.Weapons
{
    abstract class GunBase
    {
        //Public options
        public bool Firing { get; set; }

        //Protected options
        protected abstract float Range { get; }
        protected abstract float FireRatePerSecond { get; } 
        protected abstract float RecoilPerShot { get; }
        protected abstract float Damage { get; } 
        protected abstract bool AutoFire { get; }
        protected abstract Vector2f Origin { get; }
        protected abstract Vector2f AttachPoint { get; }
        protected abstract float FireSpeed { get; }
        protected abstract float FireSpeedVariation { get; }
        protected abstract float Cone { get; }

        //Helpers
        protected Vector2f OriginBarrelOffset => new Vector2f(GunSprite.Texture.Size.X - Origin.X, Origin.Y);
        public Vector2f BarrelPosition => AttachPoint + TrigHelper.FromAngleRad(LastAimAngle, GunSprite.Texture.Size.X - Origin.X);
        public bool AimingRight { get; set; }

        //Variables
        public float LastShotFromAngle { get; private set; }

        public Vector2f LastAim { get; private set; }
        public float LastAimAngle { get; private set; }
        private bool FirstShot { get; set; }
        private float FireTimer { get; set; }
        
        //Storage
        protected GameData Game { get; set; }
        private Sprite GunSprite { get; set; }

        //Events
        public event Action OnFire = delegate { };

        public GunBase(GameData game, Sprite gunSprite)
        {
            GunSprite = gunSprite;
            Game = game;
        }

        public void Aim(Vector2f location)
        {
            LastAim = Camera.ToWorld(location);
            var delta = LastAim - AttachPoint;
            AimingRight = delta.X > 0;
            LastAimAngle = delta.Angle();
        }

        public virtual void Draw(RenderWindow window)
        {
            window.Draw(GunSprite);
        }

        private void Fire()
        {
            LastShotFromAngle = LastAimAngle + (Cone * (Rng.Float() - 0.5f)); 

            if (Game.World.TouchingSolid(BarrelPosition + TrigHelper.FromAngleRad(LastShotFromAngle, 8)))  //8 is margin for big bullets
            {
                //Gun is stuck in wall
                //TODO: play sound?
            }
            else
            {
                FireBullet();
                OnFire();
            }
        }

        protected virtual void FireBullet()
        {
            var bullet = new Bullet(Game, BarrelPosition, TrigHelper.FromAngleRad(LastShotFromAngle, FireSpeed + (FireSpeedVariation * (Rng.Float() - 0.5f))), Range);
            Game.Objects.Bullets.Add(bullet);
        }

        public virtual void Update(float time)
        {
            if (FireTimer > 0)
                FireTimer -= FireRatePerSecond * time;

            if (Firing)
            {
                while((FirstShot || AutoFire) && FireTimer <= 0)
                {
                    FireTimer += 1f;
                    Fire();
                }
                FirstShot = false;
            }
            else
            {
                FirstShot = true;
            }

            GunSprite.Scale = new Vector2f(1, AimingRight ? 1 : -1);
            GunSprite.Position = AttachPoint;
            GunSprite.Origin = Origin;
            GunSprite.Rotation = TrigHelper.ToDeg(LastAimAngle);
        }
    }
}
