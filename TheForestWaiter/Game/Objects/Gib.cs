using SFML.Graphics;
using SFML.System;
using System;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Graphics;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects
{
	/// <summary>
	/// The sheet you give in the constructor will be changed (rotation/origin)
	/// </summary>
	class Gib : Movable
	{
		private const float _airRotationDrag = 0.5f;

		public SpriteSheet Sheet { get; set; }
		public int TileIndex { get; set; } = 0;
		public float AngularMomentum { get; set; } = 0;

		private float _lifeSpan = 1;
		private float _life;
		private float _rotation;

		public Gib(SpriteSheet sheet)
		{
			Sheet = sheet;
			Size = Sheet.TileSize.ToVector2f();

			Drag = new Vector2f(100, 20);
			Gravity = 500;
		}

		public void SetLife(float life)
		{
			_lifeSpan = life;
			_life = life;
		}

		public override void Update(float time)
		{
			base.Update(time);

			_life -= time;

			if (_life < 0)
			{
				Delete();
				return;
			}

			if (CollisionFlags.HasFlag(WorldCollisionFlags.Bottom))
			{
				var c = (float)(Math.Min(Sheet.TileSize.X, Sheet.TileSize.Y) * Math.PI);
				var rotate = Velocity.X * time / c * (float)Math.PI * 2f;

				_rotation += rotate;
				AngularMomentum = rotate / time;
			}
			else
			{
				_rotation += AngularMomentum * time;
				AngularMomentum = MoreMath.MoveTowardsZero(AngularMomentum, _airRotationDrag * time);
			}
		}

		public override void Draw(RenderWindow window)
		{
			Sheet.Sprite.Color = new Color(255, 255, 255, (byte)(_life / _lifeSpan * 255));
			Sheet.Sprite.Origin = (Sheet.TileSize / 2).ToVector2f();
			Sheet.Sprite.Position = Center;
			Sheet.Sprite.Rotation = TrigHelper.ToDeg(_rotation);
			Sheet.SetRect(TileIndex);
			window.Draw(Sheet);
		}
	}
}