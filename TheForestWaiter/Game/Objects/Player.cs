﻿using SFML.Graphics;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Weapons;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Objects
{
	internal class Player : GroundCreature
	{
		private const float SWITCH_COOLDOWN_TIME = 0.2f;
		private const float WALK_SOUND_INTERVAL = 0.50f;

		public PlayerController Controller { get; } = new();
		public WeaponCollection Weapons { get; } = new();

		private readonly SoundInfo _walkSound;
		private readonly SoundInfo _hurtSound;

		private readonly AnimatedSprite _sprite;
		private readonly Color _stunColor = new(255, 200, 200);
		private readonly SoundSystem _sound;
		private bool _justJumped = false;
		private float _switchCooldown = 0;
		private float _walkSoundTimer = 0;

		public Player(GameData game, ContentSource content, ObjectCreator creator, SoundSystem sound) : base(game)
		{
			_walkSound = new SoundInfo("Sounds/Player/walk_{n}.wav");
			_hurtSound = new SoundInfo("Sounds/Player/hurt.wav");
			_walkSound.Volume = 10f;
			_hurtSound.Volume = 90f;

			_sprite = content.Textures.CreateAnimatedSprite("Textures/Player/sheet.png");
			Size = _sprite.Sheet.TileSize.ToVector2f();

			StunTime = 1;
			AutoJumpObstacles = false;
			Friendly = true;
			AirAcceleration = 1500;
			AirSpeed = 250;
			JumpForceVariation = 0;
			HorizontalOverflowDrag = 100;
			InvincibleWhenStunned = true;

			Weapons.Add(creator.CreateWeapon<Handgun>());
			Weapons.OnEquipedChanged += OnEquipmentChangedEventHandler;
			this._sound = sound;
		}

		public override void Update(float time)
		{
			base.Update(time);

			var gun = Weapons.GetEquiped();

			_switchCooldown -= time;

			if (gun != null)
			{
				if (_switchCooldown <= 0)
				{
					gun.Firing = Controller.IsActive(ActionTypes.PrimaryAttack);
					gun.Aim(Controller.GetAim());
				}
				gun.Update(time);
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
			HandleSounds(time);

			foreach (var weapon in Weapons.OwnedWeapons)
			{
				weapon.BackgroundUpdate(time);
			}
		}

		public override void Draw(RenderWindow window)
		{
			if (_switchCooldown <= 0)
			{
				window.Draw(Weapons.GetEquiped());
			}

			window.Draw(_sprite);
		}

		public void God() => Invincible = true;

		private void HandleSounds(float time)
		{
			if (CollisionFlags.HasFlag(WorldCollisionFlags.Bottom) && MovingDirection != 0)
			{
				_walkSoundTimer += time;

				if (_walkSoundTimer > WALK_SOUND_INTERVAL)
				{
					_sound.Play(_walkSound);
					_walkSoundTimer = 0;
				}
			}
		}

		private void HandleAnimations(float time)
		{
			bool isMovingRight = FacingDirection > 0;
			bool isMovingLeft = FacingDirection < 0;
			bool aimingRight = TrigHelper.IsPointingRight(Controller.GetAim());

			var gun = Weapons.GetEquiped();

			if (IsStunned)
			{
				_sprite.Sprite.Color = _stunColor;
				if (gun != null) gun.Color = _stunColor;
			}
			else
			{
				if (gun != null) gun.Color = Color.White;
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

		private void OnEquipmentChangedEventHandler(Weapon previous)
		{
			previous.Firing = false;
			_switchCooldown = SWITCH_COOLDOWN_TIME;
		}

		protected override void OnDamage(GameObject by)
		{
			_sound.Play(_hurtSound);
		}

		protected override void OnDeath()
		{
			DisableDraws = true;
			DisableUpdates = true;
			EnableCollision = false;
		}
	}
}
