using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Graphics
{
	class AnimatedSprite : Drawable
	{
		public Sprite Sprite => Sheet.Sprite;

		public bool Paused { get; set; } = false;
		public bool Reversed { get; set; } = false;
		public float Framerate { get; set; } = 15;

		public List<SheetSection> Sections { get; set; } = new();

		private int _animationStart;
		public int AnimationStart
		{
			set
			{
				if (_animationStart != value)
				{
					_animationStart = value;
					CurrentFrame = 0;
				}
			}
			get => _animationStart;
		}

		private int _animationEnd;
		public int AnimationEnd
		{
			set
			{
				if (_animationEnd != value)
				{
					_animationEnd = value;
					CurrentFrame = 0;
				}
			}
			get => _animationEnd;
		}

		public int CurrentFrame
		{
			get => AnimationStart + (int)Math.Abs(Frame);
			set => Frame = value - AnimationStart;
		}
		public void SetStaticFrame(int frame)
		{
			AnimationStart = frame;
			AnimationEnd = frame;
		}

		private int Frames => AnimationEnd + 1 - AnimationStart;

		public SpriteSheet Sheet { get; private set; }
		private float Frame { get; set; } = 0;

		public AnimatedSprite(SpriteSheet sheet, int fps)
		{
			AnimationEnd = sheet.Rect.TotatlTiles - 1;
			Framerate = fps;
			Sheet = sheet;
		}

		public AnimatedSprite(Texture texture, Vector2i cellSize, int fps)
		{
			Sheet = new SpriteSheet(texture, cellSize);
			Framerate = fps;
			AnimationEnd = Sheet.Rect.TotatlTiles - 1;
		}

		public AnimatedSprite(Texture texture, Vector2i cellSize, Vector2i spacing, Vector2i margin, int fps)
		{
			Sheet = new SpriteSheet(texture, cellSize, spacing, margin);
			Framerate = fps;
			AnimationEnd = Sheet.Rect.TotatlTiles - 1;
		}

		public void SetSection(string name)
		{
			var section = Sections.First(s => s.Name == name);

			if (section.FixedFrame.HasValue)
			{
				Paused = true;
				SetStaticFrame(section.FixedFrame.Value);
			}
			else
			{
				Paused = false;
				AnimationStart = section.Start;
				AnimationEnd = section.End;
				Framerate = section.Fps;
			}
		}

		public void Update(float time)
		{
			if (!Paused)
			{
				if (!Reversed)
				{
					Frame += time * Framerate;
				}
				else
				{
					Frame -= time * Framerate;
				}
			}

			Frame %= Frames;
			Sheet.SetRect(CurrentFrame);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			Sheet.Draw(target, states);
		}
	}
}
