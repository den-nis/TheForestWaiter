using SFML.Graphics;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Weapons;
using TheForestWaiter.Game.Objects.Weapons.Guns;

namespace TheForestWaiter.Game.Objects
{
	internal class Player : GroundCreature
	{
        public PlayerController Controller { get; } = new();

		private readonly AnimatedSprite _sprite;
		private GunBase _gun;

        private readonly Color _stunColor = new(255, 200, 200);
        private bool _justJumped = false;

        public Player(GameData game, ContentSource content, ObjectCreator creator) : base(game)
		{
			_sprite = content.Textures.CreateAnimatedSprite("Textures\\Player\\sheet.png");
			Size = _sprite.Sheet.TileSize.ToVector2f();

            StunTime = 1;
            Friendly = true;
            AirAcceleration = 1500;
            AirSpeed = 100;
            JumpForceVariation = 0;

			_gun = creator.CreateGun<Handgun>();
		}

		public override void Update(float time)
		{
			base.Update(time);

			if (_gun != null)
			{
				_gun.Firing = Controller.IsActive(ActionTypes.PrimaryAttack);
				_gun.Aim(Controller.GetAim());
				_gun.Update(time);
			}

            if (Controller.IsActive(ActionTypes.Right) != Controller.IsActive(ActionTypes.Left))
            {
                if (Controller.IsActive(ActionTypes.Right))
                    MoveRight();
                if (Controller.IsActive(ActionTypes.Left))
                    MoveLeft();
            }

            if (Controller.IsActive(ActionTypes.Up) && !_justJumped)
            {
                Jump();
                _justJumped = true;
            }
            else
            {
                HoldJump();
			}

            if (!Controller.IsActive(ActionTypes.Up))
            {
                _justJumped = false;
			}

            HandleAnimations(time);
        }

		public override void Draw(RenderWindow window)
		{
            window.Draw(_gun);
			window.Draw(_sprite);
		}

        public void Equip(GunBase gun)
        {
            _gun = gun;
		}

        public void God() => Invincible = true;

        private void HandleAnimations(float time)
        {
            bool isMovingRight = MovingDirection > 0;
            bool isMovingLeft = MovingDirection < 0;
            bool aimingRight = TrigHelper.IsPointingRight(Controller.GetAim());

            if (IsStunned)
            {
                _sprite.Sprite.Color = _stunColor;
                if (_gun != null) _gun.GunSprite.Color = _stunColor;
            }
            else
            {
                if (_gun != null) _gun.GunSprite.Color = Color.White;
                _sprite.Sprite.Color = Color.White;
            }

            _sprite.Sheet.MirrorX = !aimingRight;
            _sprite.Framerate = 12;

            if ((aimingRight && isMovingRight) || (!aimingRight && isMovingLeft))
            {
                //Forward walking
                _sprite.AnimationStart = 0;
                _sprite.AnimationEnd = 6;
            }
            else
            {
                //Backward walking
                _sprite.AnimationStart = 8;
                _sprite.AnimationEnd = 15;
            }

            if (Math.Abs(GetSpeed().X) < 1f)
            {
                //Idle
                _sprite.Framerate = 5;
                _sprite.AnimationStart = 14;
                _sprite.AnimationEnd = 19;
            }

            if (!CollisionFlags.HasFlag(WorldCollisionFlags.Bottom))
                _sprite.SetStaticFrame(3);

            _sprite.Sprite.Position = Position;
            _sprite.Update(time);
        }

        protected override void OnDamage(GameObject by)
		{
		}

		protected override void OnDeath()
		{
            _gun = null;

            DisableDraws = true;
            DisableUpdates = true;
            EnableCollision = false;
        }
	}
}
