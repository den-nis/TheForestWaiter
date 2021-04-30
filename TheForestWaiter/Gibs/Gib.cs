using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entities;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Gibs
{
	/// <summary>
	/// The sheet you give in the constructor will be changed (rotation/origin)
	/// </summary>
	class Gib : DynamicObject
	{
		private const float _airRotationDrag = 0.5f;

		private readonly SpriteSheet _sheet;
		private readonly int _tileIndex;
		private readonly Vector2i _size;
		private readonly float _lifeSpan;
		private float _life;
		private float _rotation;
		public float _angularMomentum;

		public Gib(GameData game, SpriteSheet sheet, int tileIndex, float life, float initialAngularMomentum) : base(game)
		{
			_sheet = sheet;
			_tileIndex = tileIndex;
			_lifeSpan = life;
			_life = life;
			_angularMomentum = initialAngularMomentum;

			_size = _sheet.TileSize;
			Size = _size.ToVector2f();

			Gravity = 200;
			ReceiveDynamicCollisions = false;
			EmitDynamicCollisions = false;
			Drag = new Vector2f(100, 20);
		}

		public override void Update(float time)
		{
			_life -= time;

			if (_life < 0)
				MarkedForDeletion = true;

			if (TouchingFloor)
			{ 
				var c = (float)(Math.Min(_size.X, _size.Y) * Math.PI);
				var rotate = velocity.X * time / c * (float)Math.PI * 2f;

				_rotation += rotate;
				_angularMomentum = rotate / time;
			}
			else
			{
				_rotation += _angularMomentum * time;
				_angularMomentum = ForestMath.MoveTowardsZero(_angularMomentum, _airRotationDrag * time);
			}

			base.Update(time);
		}

		public override void Draw(RenderWindow window)
		{
			_sheet.Sprite.Color = new Color(255, 255, 255, (byte)(_life / _lifeSpan * 255));
			_sheet.Sprite.Origin = (_size / 2).ToVector2f();
			_sheet.Sprite.Position = Center;
			_sheet.Sprite.Rotation = TrigHelper.ToDeg(_rotation);
			_sheet.SetRect(_tileIndex);
			window.Draw(_sheet);
		}
	}
}
