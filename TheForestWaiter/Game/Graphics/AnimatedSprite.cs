using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheForestWaiter.Shared;

namespace TheForestWaiter.Game.Graphics
{
    class AnimatedSprite : Drawable
    {
        public Sprite Sprite => Sheet.Sprite;

        public bool Paused { get; set; } = false;
        public bool Reversed { get; set; } = false;
        public int Framerate { get; set; } = 15;

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
            AnimationEnd = sheet.TotatlTiles-1;
            Framerate = fps;
            Sheet = sheet;
        }

        public AnimatedSprite(Texture texture, int cellWidth, int cellHeight, int fps)
        {
            Sheet = new SpriteSheet(texture, cellWidth, cellHeight);
            Framerate = fps;
            AnimationEnd = Sheet.TotatlTiles-1;
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
