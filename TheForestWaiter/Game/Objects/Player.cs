using SFML.Graphics;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Objects.Weapons;
using TheForestWaiter.Game.Objects.Weapons.Abstract;
using TheForestWaiter.Game.Objects.Weapons.Guns;

namespace TheForestWaiter.Game.Objects
{
	internal class Player : GroundCreature
	{
        public PlayerController Controller { get; } = new();

		private readonly AnimatedSprite _sprite;
		private ProjectileLauncher _gun;

        private readonly Color _stunColor = new(255, 200, 200);
        private bool _justJumped = false;

        public Player(GameData game, ContentSource content, ObjectCreator creator) : base(game)
		{
			_sprite = content.Textures.CreateAnimatedSprite("Textures/Player/sheet.png");
			Size = _sprite.Sheet.TileSize.ToVector2f();

            StunTime = 1;
            AutoJumpObstacles = false;
            Friendly = true;
            AirAcceleration = 1500;
            AirSpeed = 100;
            JumpForceVariation = 0;
            HorizontalOverflowDrag = 100;

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

        public void Equip(ProjectileLauncher gun)
        {
            _gun = gun;
		}

        public void God() => Invincible = true;

        private void HandleAnimations(float time)
        {
            bool isMovingRight = FacingDirection > 0;
            bool isMovingLeft = FacingDirection < 0;
            bool aimingRight = TrigHelper.IsPointingRight(Controller.GetAim());

            if (IsStunned)
            {
                _sprite.Sprite.Color = _stunColor;
                if (_gun != null) _gun.Color = _stunColor;
            }
            else
            {
                if (_gun != null) _gun.Color = Color.White;
                _sprite.Sprite.Color = Color.White;
            }

            _sprite.Sheet.MirrorX = !aimingRight;

            if ((aimingRight && isMovingRight) || (!aimingRight && isMovingLeft))
            {
                _sprite.SetSection("walking");
            }
            else
            {
                _sprite.SetSection("backwards");
            }

            if (Math.Abs(GetSpeed().X) < 1f)
            {
                _sprite.SetSection("idle");
            }

            if (!CollisionFlags.HasFlag(WorldCollisionFlags.Bottom))
            {
                _sprite.SetSection("flying");
			}

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
