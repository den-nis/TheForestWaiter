using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Objects;

namespace TheForestWaiter.Game.Gibs
{
	internal class Coin : PhysicsObject
	{
		private const float LIFESPAN = 20;
		private float _life = LIFESPAN;

		private readonly Sprite _sprite;

		public Coin(GameData game, ContentSource content) : base(game)
		{
			_sprite = content.Textures.CreateSprite("Textures\\Items\\coin.png");
			Size = new Vector2i(_sprite.TextureRect.Width, _sprite.TextureRect.Height).ToVector2f();
			CollisionRadius = 20;
			ReceivePhysicsCollisions = true;
			EmitPhysicsCollisions = true;
			EnableDrag = true;
			Drag = new Vector2f(100, 0);
			Gravity = 200;
		}

		public override void Update(float time)
		{
			_life -= time;

			if (_life < 0)
			{
				MarkedForDeletion = true;
				return;
			}

			if (TouchingFloor)
			{
				SetVelocityY(Rng.Range(-120, -90));
			}

			PhysicsTick(time);
			_sprite.Position = Position;
		}

		protected override void OnTouch(PhysicsObject obj)
		{
			if (obj is Player)
			{
				MarkedForDeletion = true;
				Game.Session.Coins++;
			}
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_sprite);
		}
	}
}