using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Entities;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;
using TheForestWaiter.Objects.Weapons;
using TheForestWaiter.Objects.Weapons.Guns;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Objects
{
    class Player : Creature
    {
        private readonly Color _stunColor = new(255,200,200);

        public AnimatedSprite AnimatedSprite { get; set; } 

        private GunBase Gun { get; set; }
        private float EarlyJumpTime { get; set; } = (float)TimeSpan.FromSeconds(0.1f).TotalSeconds;

        private bool Jumping { get; set; } = false;
        private float EarlyJumpTimer { get; set; }
        private bool MovingRight { get; set; } = false;
        private bool MovingLeft { get; set; } = false;
        private Vector2f _aim;
        private bool _aimingRight = true;

        public Player(GameData game) : base(game)
        {
            Health = 100;
            AnimatedSprite = GameContent.Textures.CreateAnimatedSprite("Textures\\Player\\sheet.png");
            Gun = new Sniper(game);
            Size = AnimatedSprite.Sheet.TileSize.ToVector2f();
            CollisionRadius = Size.Y + 5;
            ReceiveDynamicCollisions = true;
            EmitDynamicCollisions = false;
            Gravity = 1000;
        }

        public override void Draw(RenderWindow window)
        {
            if (!Dead)
            {
                Gun?.Draw(window);
                window.Draw(AnimatedSprite.Sheet.Sprite);
            }
        }

        public void StartFire()
        {
            if (Gun != null)
                Gun.Firing = true;
        }

        public void StopFire()
        {
            if (Gun != null)
			    Gun.Firing = false;
        }

        public void Aim(Vector2f mouse)
        {
            _aim = mouse;
            _aimingRight = mouse.X > Camera.ToCamera(Center).X;
            Gun?.Aim(mouse);
        }

        public void StartMoveRight() => MovingRight = true;
        public void StopMoveRight() => MovingRight = false;
        public void StartMoveLeft() => MovingLeft = true;
        public void StopMoveLeft() => MovingLeft = false;

        public void StartJump()
        {
            Jumping = true;
            EarlyJumpTimer = EarlyJumpTime;
        }

        public void StopJump() => Jumping = false;

        private void HandleMovement(float time)
        {
            if (Dead)
            {
                velocity = default;
                return;
            }

            if (IsStunned)
                return;

            if (EarlyJumpTimer > 0 && TouchingFloor)
                velocity.Y = -350;
            EarlyJumpTimer -= time;

            if (Jumping)
                velocity.Y -= time * 300;

            velocity.X = 0;
            if (MovingRight && !MovingLeft)
                velocity.X = 200;

            if (MovingLeft && !MovingRight)
                velocity.X = -200;
        }

        public void HandleAnimations(float time)
        {
            if (IsStunned)
            {
                AnimatedSprite.Sprite.Color = _stunColor;
                if (Gun != null) Gun.GunSprite.Color = _stunColor;
            }
            else
            {
                if (Gun != null) Gun.GunSprite.Color = Color.White;
                AnimatedSprite.Sprite.Color = Color.White;
            }

            var isMovingRight = RealSpeed.X > 1;
            var isMovingLeft = RealSpeed.X < -1;

            AnimatedSprite.Sheet.MirrorX = !_aimingRight;
            AnimatedSprite.Framerate = 12;

            if ((_aimingRight && isMovingRight) || (!_aimingRight && isMovingLeft))
            {
                //Forward walking
                AnimatedSprite.AnimationStart = 0;
                AnimatedSprite.AnimationEnd = 6;
            }
            else
            {
                //Backward walking
                AnimatedSprite.AnimationStart = 8;
                AnimatedSprite.AnimationEnd = 15;
            }
            
            if (Math.Abs(RealSpeed.X) < 1f)
            {
                //Idle
                AnimatedSprite.Framerate = 5;
                AnimatedSprite.AnimationStart = 14;
                AnimatedSprite.AnimationEnd = 19;
            }

            if (!TouchingFloor)
                AnimatedSprite.SetStaticFrame(3);
            
            AnimatedSprite.Sprite.Position = Position;
            AnimatedSprite.Update(time);
        }

        public override void Update(float time)
        {
            Gun?.Update(time);
            HandleMovement(time);
            HandleAnimations(time);
            base.Update(time);
        }
         
        public void RemoveGun() => SetGun(null);

        public void SetGun(GunBase gun)
        {
            if (gun != null)
            {
                gun.Aim(_aim);
			}

            Gun = gun;
        }

        protected override void OnDeath()
        {
            RemoveGun();
            Gravity = 0;
        }

        protected override void OnDamage(DynamicObject by)
        {
            if (by != null) 
                ApplyKnockback(by);
        }
    }
}