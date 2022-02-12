using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Objects.Weapons;
using TheForestWaiter.Game.Objects.Weapons.Guns;

namespace TheForestWaiter.Game.Objects
{
    internal class Player : Creature
    {
        private const int MAX_HEALTH = 100;
        private const float JUMP_VELOCITY = 380;
        private const float WALK_SPEED = 260;
        private const float EARLY_JUMP_TIME = 100 * Time.MILLISECONDS;

        public GunBase Gun { get; private set; }

        private readonly AnimatedSprite _animatedSprite;
        private readonly Color _stunColor = new(255, 200, 200);
        private readonly Camera _camera;

        private float _earlyJumpTimer;

        private bool _jumping = false;
        private bool _moveRight = false;
        private bool _moveLeft = false;
        private bool _aimingRight = true;

        private Vector2f _aim;

        public Player(GameData game, ContentSource content, ObjectCreator creator, Camera camera) : base(game)
        {
            Health = MAX_HEALTH;
            _animatedSprite = content.Textures.CreateAnimatedSprite("Textures\\Player\\sheet.png");
            Gun = creator.CreateGun<Handgun>();

            Size = _animatedSprite.Sheet.TileSize.ToVector2f();
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
                window.Draw(_animatedSprite.Sheet.Sprite);
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

        public void StartMoveRight() => _moveRight = true;
        public void StopMoveRight() => _moveRight = false;
        public void StartMoveLeft() => _moveLeft = true;
        public void StopMoveLeft() => _moveLeft = false;

        public void StartJump()
        {
            _jumping = true;
            _earlyJumpTimer = EARLY_JUMP_TIME;
        }

        public void StopJump() => _jumping = false;

        public void Heal(int points)
        {
            Health += points;
            Health = Math.Min(Health, MAX_HEALTH);
		}

        private void HandleMovement(float time)
        {
            if (Dead)
            {
                Velocity = default;
                return;
            }

            if (TouchingFloor)
            {
                if (_earlyJumpTimer > 0)
                    SetVelocityY(-JUMP_VELOCITY);
            }
            _earlyJumpTimer -= time;

            if (_jumping)
                Push(new Vector2f(0, -300), time);

            if (_moveRight && !_moveLeft)
                LimitPush(new Vector2f(WALK_SPEED, 0), 1);

            if (_moveLeft && !_moveRight)
                LimitPush(new Vector2f(-WALK_SPEED, 0), 1);

            EnableDrag = !_moveLeft && !_moveRight;
        }

        private void HandleAnimations(float time)
        {
            if (IsStunned)
            {
                _animatedSprite.Sprite.Color = _stunColor;
                if (Gun != null) Gun.GunSprite.Color = _stunColor;
            }
            else
            {
                if (Gun != null) Gun.GunSprite.Color = Color.White;
                _animatedSprite.Sprite.Color = Color.White;
            }

            var isMovingRight = RealSpeed.X > 1;
            var isMovingLeft = RealSpeed.X < -1;

            _animatedSprite.Sheet.MirrorX = !_aimingRight;
            _animatedSprite.Framerate = 12;

            if ((_aimingRight && isMovingRight) || (!_aimingRight && isMovingLeft))
            {
                //Forward walking
                _animatedSprite.AnimationStart = 0;
                _animatedSprite.AnimationEnd = 6;
            }
            else
            {
                //Backward walking
                _animatedSprite.AnimationStart = 8;
                _animatedSprite.AnimationEnd = 15;
            }

            if (Math.Abs(RealSpeed.X) < 1f)
            {
                //Idle
                _animatedSprite.Framerate = 5;
                _animatedSprite.AnimationStart = 14;
                _animatedSprite.AnimationEnd = 19;
            }

            if (!TouchingFloor)
                _animatedSprite.SetStaticFrame(3);

            _animatedSprite.Sprite.Position = Position;
            _animatedSprite.Update(time);
        }

        public override void Update(float time)
        {
            PhysicsTick(time);
            HandleMovement(time);

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

            DisableDraws = true;
            DisableUpdates = true;
            ReceivePhysicsCollisions = false;
        }

        protected override void OnDamage(PhysicsObject by)
        {
            if (by != null)
                ApplyStunMovement(by);
        }
    }
}