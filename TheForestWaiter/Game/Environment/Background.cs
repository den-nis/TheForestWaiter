using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Environment
{
	sealed internal class Background
	{
		public float Horizon { get; set; } = 0;

		private const float SKY_HEIGHT_FOCUS = 540f;
		private const float STARS_HEIGHT_FOCUS = 600f;
		private const float MOUNTAIN_HEIGHT_FOCUS = 250f;
		private const float TREES_HEIGHT_FOCUS = 50f;

		private const float VERTICAL_MULTIPLIER = 0.9f;

		private const float SKY_SPEED = 0.05f;
		private const float STARS_SPEED = 0.1f;
		private const float MOUNTAIN_SPEED = 0.2f;
		private const float TREES_SPEED = 0.5f;

		private readonly Sprite _spriteSky;
		private readonly Sprite _spriteStars;
		private readonly Sprite _spriteMountain;
		private readonly Sprite _spriteTrees;

		private readonly Sprite[] _all;
        private readonly Camera _camera;
        private Vector2f _offset;
		private Vector2f _windowSize;

		public Background(ContentSource content, Camera camera)
		{
            _camera = camera;
			_spriteSky = content.Textures.CreateSprite("Textures/Background/sky.png");
			_spriteStars = content.Textures.CreateSprite("Textures/Background/stars.png");
			_spriteMountain = content.Textures.CreateSprite("Textures/Background/mountains.png");
			_spriteTrees = content.Textures.CreateSprite("Textures/Background/trees.png");

			_all = new Sprite[] { 
				_spriteSky, 
				_spriteStars, 
				_spriteMountain,
				_spriteTrees, 
			};

			foreach(var i in _all)
			{
				i.Texture.Repeated = true;
			}

			//TODO: is this too much logic for the constructor?
			UpdateSize((int)camera.MaxWorldView.X, (int)camera.MaxWorldView.Y);
        }

		public void UpdateSize(int width, int height)
		{
			_windowSize = new Vector2f(width, height);

			foreach (var sprite in _all)
			{
				sprite.TextureRect = new IntRect(sprite.TextureRect.Left, sprite.TextureRect.Top, width, sprite.TextureRect.Height);
			}
		}

		public void SetOffset(Vector2f offset)
		{
			_offset = new Vector2f(offset.X, -(Horizon - offset.Y));
		}

		public void Draw(RenderWindow window)
		{
			window.SetView(Camera.GetWindowView(window));

			foreach (var i in _all)
			{
				window.Draw(i);
			}

			window.SetView(_camera.GetView());
		}

		public void Update()
		{
			var center = _windowSize.Y / 2;

			static void SetSpriteOffset(Sprite sprite, Vector2f offset)
			{
				sprite.TextureRect = new IntRect(
					(int)offset.X,
					(int)offset.Y,
					sprite.TextureRect.Width,
					sprite.TextureRect.Height
				);
			}

			SetSpriteOffset(_spriteSky, new Vector2f(_offset.X * SKY_SPEED, 0));
			SetSpriteOffset(_spriteStars, new Vector2f(_offset.X * STARS_SPEED, 0));
			SetSpriteOffset(_spriteMountain, new Vector2f(_offset.X * MOUNTAIN_SPEED, 0));
			SetSpriteOffset(_spriteTrees, new Vector2f(_offset.X * TREES_SPEED, 0));

			_spriteSky.Position      = new Vector2f(_spriteSky.Position.X,      (center - SKY_HEIGHT_FOCUS)      - (_offset.Y * SKY_SPEED      * VERTICAL_MULTIPLIER));
			_spriteStars.Position    = new Vector2f(_spriteStars.Position.X,    (center - STARS_HEIGHT_FOCUS)    - (_offset.Y * STARS_SPEED    * VERTICAL_MULTIPLIER));
			_spriteMountain.Position = new Vector2f(_spriteMountain.Position.X, (center - MOUNTAIN_HEIGHT_FOCUS) - (_offset.Y * MOUNTAIN_SPEED * VERTICAL_MULTIPLIER));
			_spriteTrees.Position    = new Vector2f(_spriteTrees.Position.X,    (center - TREES_HEIGHT_FOCUS)    - (_offset.Y * TREES_SPEED    * VERTICAL_MULTIPLIER));
		}
	}
}
