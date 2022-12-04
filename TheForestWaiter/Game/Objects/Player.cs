using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Gibs;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Weapons.Abstract;
using TheForestWaiter.Graphics;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.Multiplayer.Messages;
using TheForestWaiter.States;
using TheForestWaiter.UI.Menus;

namespace TheForestWaiter.Game.Objects;

internal class Player : GroundCreature
{
	private const float TIME_TILL_DEATH_SCREEN = 4f;
	private const float SWITCH_COOLDOWN_TIME = 0.2f;
	private const float WALK_SOUND_INTERVAL = 0.50f;

	public PlayerController Controller { get; } = new();
	public Inventory Inventory { get; }

	private readonly NetContext _network;
	private readonly Camera _camera;
	private readonly AnimatedSprite _sprite;
	private readonly ContentSource _content;
	private readonly SoundSystem _sound;
	private readonly GibSpawner _gibs;

	private readonly SoundInfo _walkSound;
	private readonly SoundInfo _deathSound;

	private readonly Color _stunColor = new(255, 200, 200);

	private bool _justJumped = false;
	private float _switchCooldown = 0;
	private float _walkSoundTimer = 0;
	private Vector2f _lastPosition = default;

	public Player()
	{
		IsClientSide = true;

		var creator = IoC.GetInstance<ObjectCreator>();
		_network = IoC.GetInstance<NetContext>();
		_content = IoC.GetInstance<ContentSource>();
		_sound = IoC.GetInstance<SoundSystem>();
		_gibs = IoC.GetInstance<GibSpawner>();
		_camera = IoC.GetInstance<Camera>();

		Inventory = IoC.GetInstance<Inventory>();
		Inventory.Owner = this;

		_gibs.Setup("Textures/Player/player_gibs.png");
		_gibs.MaxVelocity = 500;

		SoundOnDamage = new("Sounds/Player/hurt.wav") { Volume = 90f };
		_walkSound = new("Sounds/Player/walk_{n}.wav") { Volume = 10f };
		_deathSound = new("Sounds/Player/death.wav");

		_sprite = _content.Textures.CreateAnimatedSprite("Textures/Player/sheet.png");
		Size = _sprite.Sheet.TileSize.ToVector2f();

		StunTime = 1;
		AutoJumpObstacles = false;
		Friendly = true;
		AirAcceleration = 1500;
		AirSpeed = 250;
		JumpForceVariation = 0;
		HorizontalOverflowDrag = 200;
		InvincibleWhenStunned = true;
		JumpHoldForce = 300;
		JumpForce = 400;

		Inventory.OnEquipWeapon += OnWeaponChangedEventHandler;
		Inventory.Add(1);

		if (_network.Settings.IsMultiplayer)
		{
			//TODO: every ghost is subscribed to these events. 
			//Can't put the IsGhost check outside the event handler because it's assigned after the constructor has run
			Controller.OnAction += (action, toggle) => 
			{
				if (!IsGhost && Alive)
					_network.Traffic.Send(new PlayerAction { Action = action, State = toggle, SharedId = SharedId });
			};

			Controller.OnAim += (angle) => 
			{
				if (!IsGhost && Alive)
					_network.Traffic.Send(new PlayerAim { Angle = angle, SharedId = SharedId });
			};

			Inventory.OnEquipedChanged += (previous) => 
			{
				if (!IsGhost && Alive)
					_network.Traffic.Send(new PlayerItems
					{
						SharedId = SharedId,
						Items = Inventory.Items.ToArray(),
						EquipedIndex = Inventory.EquipedIndex,
					});
			};
		}
	}

	public override void Update(float time)
	{
		base.Update(time);

		var gun = Inventory.GetCurrentWeapon();

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

		if (Controller.IsActive(ActionTypes.Up))
		{
			if (!_justJumped)
			{
				Jump();
				_justJumped = true;
			}
			else
			{
				HoldJump();

			}
		}

		if (!Controller.IsActive(ActionTypes.Up))
		{
			_justJumped = false;
		}

		HandleAnimations(time);
		HandleSounds(time);

		foreach (var weapon in Inventory.Weapons)
		{
			weapon.BackgroundUpdate(time);
		}

		if (_network.Settings.IsMultiplayer && !IsGhost && _lastPosition != Position && Alive)
		{
			//TODO:
			//I could probably get away with sending this less often than everything frame.
			//It can fill in the blank using the controller actions.
			_network.Traffic.Send(new PlayerPosition
			{
				X = Position.X,
				Y = Position.Y,
				SharedId = SharedId, 
			});
		}

		_lastPosition = Position;
	}

	public override void Draw(RenderWindow window)
	{
		if (_switchCooldown <= 0)
		{
			var weapon = Inventory.GetCurrentWeapon();
			if (weapon != null) window.Draw(weapon);
		}

		window.Draw(_sprite);
	}

	public void God() => Invincible = true;

	private void HandleSounds(float time)
	{
		if (CollisionFlags.HasFlag(WorldCollisionFlags.Bottom) && MovingDirection != 0)
		{
			_walkSoundTimer -= time;

			if (_walkSoundTimer <= 0)
			{
				_sound.Play(_walkSound);
				_walkSoundTimer = WALK_SOUND_INTERVAL;
			}
		}
		else
		{
			_walkSoundTimer = 0;
		}
	}

	private void HandleAnimations(float time)
	{
		bool isMovingRight = FacingDirection > 0;
		bool isMovingLeft = FacingDirection < 0;
		bool aimingRight = TrigHelper.IsPointingRight(Controller.GetAim());

		var gun = Inventory.GetCurrentWeapon();

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

	private void OnWeaponChangedEventHandler(Weapon previous)
	{
		if (previous != null)
		{
			previous.Firing = false;
		}

		_switchCooldown = SWITCH_COOLDOWN_TIME;

		var current = Inventory.GetCurrentWeapon();
		if (current != null)
		{
			CarryingWeight = current.Weight;
		}
	}

	protected override void OnDamage(GameObject by, float amount)
	{
	}

	protected override void OnDeath()
	{
		_sound.Play(_deathSound);

		DisableDraws = true;
		DisableUpdates = true;
		EnableCollision = false;

		if (!IsGhost)
		{
			if (!_network.Settings.IsMultiplayer)
			{
				IoC.GetInstance<PlayStats>().Harvest();

				IoC.GetInstance<StateManager>().StartTransition(new StateTransition
				{
					Delay = TIME_TILL_DEATH_SCREEN,
					Length = 5,
					TargetState = typeof(DeathMenu)
				}, true);
			}
			else
			{
				//Go to spectator mode
				_camera.SpectatorMode = true;
				_camera.Focus = Game.Objects.Players.FirstOrDefault(p => p.Alive);
			}
		}

		_gibs.SpawnAll(Center);
		var prop = _content.Particles.Get("Particles/blood.particle", Center);
		Game.Objects.WorldParticles.Emit(prop, 50);
	}

	public IEnumerable<IMessage> GenerateInfoMessages()
	{
		yield return new PlayerPosition { SharedId = SharedId, X = Position.X, Y = Position.Y };
		yield return new PlayerAim { SharedId = SharedId, Angle = Controller.GetAim() };
		
		foreach (var action in Enum.GetValues<ActionTypes>())
		{
			yield return new PlayerAction
			{
				SharedId = SharedId,
				Action = action,
				State = Controller.IsActive(action),
			};
		}

		yield return new PlayerItems
		{
			SharedId = SharedId,
			Items = Inventory.Items.ToArray(),
			EquipedIndex = Inventory.EquipedIndex,
		};
	}
}
