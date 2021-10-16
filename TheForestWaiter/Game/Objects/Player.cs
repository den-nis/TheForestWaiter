using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Objects.Weapons;
using TheForestWaiter.Game.Objects.Weapons.Guns;

namespace TheForestWaiter.Game.Objects
{
    class Player : Creature
    {
        private const float SPEED_UP_PER_SECOND = 50;
        private const float SECONDS_OF_SPEED_UP = 4;
        private const float MAX_NORMAL_SPEED = 200;

        private readonly Color _stunColor = new(255,200,200);
        private readonly Camera _camera;

        public AnimatedSprite AnimatedSprite { get; set; }
        public GunBase Gun { get; private set; }

        private float EarlyJumpTime { get; set; } = (float)TimeSpan.FromSeconds(0.1f).TotalSeconds;
        private float EarlyJumpTimer { get; set; }

        private bool Jumping { get; set; } = false;
        private bool MoveRight { get; set; } = false;
        private bool MoveLeft { get; set; } = false;
        private Vector2f _aim;
        private bool _aimingRight = true;
        private float _speedBuildUp = 0;

        public Player(GameData game, ContentSource content, ObjectCreator creator, Camera camera) : base(game)
        {
            Health = 100;
            AnimatedSprite = content.Textures.CreateAnimatedSprite("Textures\\Player\\sheet.png");
            Gun = creator.CreateGun<Handgun>();

            Size = AnimatedSprite.Sheet.TileSize.ToVector2f();
            CollisionRadius = Size.Y + 5;
            ReceivePhysicsCollisions = true;
            EmitPhysicsCollisions = false;
            _camera = camera;
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
            _aimingRight = mouse.X > _camera.ToCamera(Center).X;
            Gun?.Aim(mouse);
        }

        public void StartMoveRight() => MoveRight = true;
        public void StopMoveRight() => MoveRight = false;
        public void StartMoveLeft() => MoveLeft = true;
        public void StopMoveLeft() => MoveLeft = false;

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
                Velocity = default;
                return;
            }

            if (TouchingFloor)
            {
                HandleSpeedBuildUp(time);

                if (EarlyJumpTimer > 0)
                    SetVelocityY(-350);
            }
            EarlyJumpTimer -= time;

            if (Jumping)
                Push(new Vector2f(0, -300), time);

            if (MoveRight && !MoveLeft)
                LimitPush(new Vector2f(MAX_NORMAL_SPEED + _speedBuildUp * SPEED_UP_PER_SECOND, 0), 1);

            if (MoveLeft && !MoveRight)
                LimitPush(new Vector2f(-MAX_NORMAL_SPEED + _speedBuildUp * SPEED_UP_PER_SECOND, 0), 1);

            EnableDrag = !MoveLeft && !MoveRight;
        }

        private void HandleSpeedBuildUp(float time)
		{
            if (RealSpeed.X > -1 && RealSpeed.X < 1)
                _speedBuildUp = 0;

            if (RealSpeed.X > 1 || MoveRight)
                _speedBuildUp = Math.Min(SECONDS_OF_SPEED_UP, Math.Max(0, _speedBuildUp + time));
            
            if (RealSpeed.X < -1 || MoveLeft)
                _speedBuildUp = Math.Max(-SECONDS_OF_SPEED_UP, Math.Min(0, _speedBuildUp - time));

        }

        private void HandleAnimations(float time)
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
			HandleMovement(time);
            PhysicsTick(time);

            //Gun needs to updated after physics tick since it needs the latest player position
            Gun?.Update(time);
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

        protected override void OnDamage(PhysicsObject by)
        {
            if (by != null) 
                ApplyStunMovement(by);
        }
    }
}