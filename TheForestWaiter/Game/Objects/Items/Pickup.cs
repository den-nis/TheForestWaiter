using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Objects.Items
{
	internal abstract class Pickup : PhysicsObject
	{
		private const int MAX_BOUNCH_HEIGHT = 120;
		private const int MIN_BOUNCH_HEIGHT = 90;

		private const int PLAYER_ATTRACT_DISTANCE = 100;
		private const int PLAYER_ATTRACT_VELOCITY = 410;

		private const int LIFE_SPAN = 60;
		private const int TIME_WHEN_VISIBLE_DECAY = 10;

		private readonly AnimatedSprite _animation;
		private float _life = LIFE_SPAN;

		public Pickup(string texture, GameData game, ContentSource content) : base(game)
		{
			_animation = content.Textures.CreateAnimatedSprite(texture);
			_animation.CurrentFrame = Rng.RangeInt(_animation.AnimationStart, _animation.AnimationEnd);

			Size = _animation.Sheet.TileSize.ToVector2f();
			Drag = new Vector2f(100, 0);

			Gravity = 200;
			CollisionRadius = Math.Max(Size.X * 2, Size.Y * 2);
			ReceivePhysicsCollisions = true;
			EmitPhysicsCollisions = true;
			EnableDrag = true;
		}

		public override void Update(float time)
		{
			PhysicsTick(time);
			_life -= time;

			if (_life < 0)
			{
				MarkedForDeletion = true;
				return;
			}

			if (TouchingFloor)
			{
				SetVelocityY(Rng.Range(-MIN_BOUNCH_HEIGHT, -MAX_BOUNCH_HEIGHT));
			}

			if ((Game.Objects.Player.Center - Center).Len() < PLAYER_ATTRACT_DISTANCE)
			{
				var direction = (Game.Objects.Player.Center - Center).Angle();
				Velocity = TrigHelper.FromAngleRad(direction, PLAYER_ATTRACT_VELOCITY);
			}

			if (_life < TIME_WHEN_VISIBLE_DECAY)
			{
				_animation.Sprite.Color = new Color(255, 255, 255, (byte)(_life / TIME_WHEN_VISIBLE_DECAY * 255));
			}

			_animation.Update(time);
			_animation.Sheet.Sprite.Position = Position;
		}

		protected sealed override void OnTouch(PhysicsObject obj)
		{
			if (obj is Player player)
			{
				OnPickup(player);
				MarkedForDeletion = true;
			}
		}

		protected abstract void OnPickup(Player player);

		public override void Draw(RenderWindow window)
		{
			window.Draw(_animation);
		}
	}
}
