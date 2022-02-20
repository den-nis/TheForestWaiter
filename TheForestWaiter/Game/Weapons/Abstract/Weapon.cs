using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Weapons.Abstract
{
	internal abstract class Weapon : Drawable
    {
        public bool Firing { get; set; }
        public Color Color { get; set; }

        public abstract string IconTextureName { get; }
        protected abstract Vector2f Origin { get; }
        protected abstract Vector2f AttachPoint { get; }

        protected bool AutoFire { get; set; } = true;
        protected float FireSpeed { get; set; } = 1000;
        protected float FireRatePerSecond { get; set; } = 10;
        protected float FireSpeedVariation { get; set; } = 0;
        protected float KickbackForce { get; set; } = 0;
        protected float Cone { get; set; } = 0;

        protected Vector2f OriginBarrelOffset => new(Sprite.Texture.Size.X - Origin.X, Origin.Y);
        protected Vector2f BarrelPosition => AttachPoint + TrigHelper.FromAngleRad(LastAimAngle, Sprite.Texture.Size.X - Origin.X);
        protected int AimingDirection { get; private set; } = 1;

        protected GameData Game { get; set; }
        protected Sprite Sprite { get; set; }

        public float LastShotFromAngle { get; private set; }
        public Vector2f LastAim { get; private set; }
        public float LastAimAngle { get; private set; }

        private readonly ObjectCreator _creator;

        private float _fireTimer;
        private bool _firstShot;

        public Weapon(GameData game, ObjectCreator creator)
        {
            _creator = creator;
            Game = game;
        }

        public abstract void OnFire();

        public void Aim(float angle)
        {
            AimingDirection = TrigHelper.IsPointingRight(angle) ? 1 : -1;
            LastAimAngle = angle;
        }

        public virtual void Draw(RenderWindow window)
        {
            window.Draw(Sprite);
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
                OnFire();
                Kickback();
            }
        }

        public virtual void Update(float time)
        {
            Sprite.Color = Color;

            if (_fireTimer > 0)
                _fireTimer -= FireRatePerSecond * time;

            if (Firing)
            {
                while((_firstShot || AutoFire) && _fireTimer <= 0)
                {
                    _fireTimer += 1f;
                    Fire();
                }
                _firstShot = false;
            }
            else
            {
                _firstShot = true;
            }

            Sprite.Scale = new Vector2f(1, AimingDirection);
            Sprite.Position = AttachPoint;
            Sprite.Origin = Origin;
            Sprite.Rotation = TrigHelper.ToDeg(LastAimAngle);
        }

		public void Draw(RenderTarget target, RenderStates states)
		{
            target.Draw(Sprite);
		}

        private void Kickback()
        {
            Game.Objects.Player.Velocity += TrigHelper.FromAngleRad((float)(LastAimAngle - Math.PI), KickbackForce);
        }

        protected void FireProjectile<T>() where T : Projectile
        {
            var velocity = TrigHelper.FromAngleRad(LastShotFromAngle, Rng.Var(FireSpeed, FireSpeedVariation));
            var bullet = _creator.FireProjectile<T>(BarrelPosition, velocity, Game.Objects.Player);
            Game.Objects.Projectiles.Add(bullet);
        }
    }
}
