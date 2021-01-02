using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Debugging;
using TheForestWaiter.Entites;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Extensions;
using TheForestWaiter.Graphics;
using TheForestWaiter.Objects.Weapons;
using TheForestWaiter.Objects.Weapons.Guns;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Objects
{
    public class Player : DynamicObject
    {
        public AnimatedSprite AnimatedSprite { get; set; } 
        private GunBase Gun { get; set; }

        public TimeSpan EarlyJumpTime { get; set; } = TimeSpan.FromSeconds(0.1f);

        private bool Jumping { get; set; } = false;
        private float EarlyJumpTimer { get; set; } = 0;
        private bool MovingRight { get; set; } = false;
        private bool MovingLeft { get; set; } = false;
 
        public Player(GameData game) : base(game)
        {
            AnimatedSprite = new AnimatedSprite(new SpriteSheet(GameContent.GetTexture("Content.Textures.Player.player_running.png"), 17, 36), 12);
            Gun = new SmallGun(game);
            Size = new Vector2f(17, 36);
            Gravity = 1000;
        }

        public override void Draw(RenderWindow window)
        {
            Gun.Draw(window);
            window.Draw(AnimatedSprite.Sheet.Sprite);
        }

        public void StartFire() => Gun.Firing = true;
        public void StopFire() => Gun.Firing = false;
        public void StartMoveRight() => MovingRight = true;
        public void StopMoveRight() => MovingRight = false;
        public void StartMoveLeft() => MovingLeft = true;
        public void StopMoveLeft() => MovingLeft = false;

        public void StartJump()
        {
            Jumping = true;
            EarlyJumpTimer = (float)EarlyJumpTime.TotalSeconds;
        }
        public void StopJump() => Jumping = false;

        public void Aim(Vector2f mouse)
        {
            Gun.Aim(mouse);
        }

        private void HandleMovement(float time)
        {
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
            var isMovingRight = RealSpeed.X > 1;
            var isMovingLeft = RealSpeed.X < -1;

            AnimatedSprite.Sheet.MirrorX = !Gun.AimingRight;
            AnimatedSprite.Framerate = 12;

            if ((Gun.AimingRight && isMovingRight) || (!Gun.AimingRight && isMovingLeft))
            {
                //Forward walking
                AnimatedSprite.AnimationStart = 1;
                AnimatedSprite.AnimationEnd = 7;
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
                AnimatedSprite.AnimationStart = 16;
                AnimatedSprite.AnimationEnd = 20;
            }

            if (!TouchingFloor)
                AnimatedSprite.SetSingleFrame(4);
            
            AnimatedSprite.Sprite.Position = Position;
            AnimatedSprite.Update(time);
        }

        public override void Update(float time)
        {
            Gun.Update(time);
            HandleMovement(time);
            HandleAnimations(time);
            base.Update(time);
        }
    }
}