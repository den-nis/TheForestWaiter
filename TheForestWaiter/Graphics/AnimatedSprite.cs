﻿using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Graphics
{
    class AnimatedSprite : Drawable
    {
        public Sprite Sprite => Sheet.Sprite;

        public bool Reversed { get; set; } = false;
        public int Framerate { get; set; } = 15;
        public int AnimationStart { get; set; } = 1;
        public int AnimationEnd { get; set; }
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
            AnimationEnd = sheet.TotatlCells;
            Framerate = fps;
            Sheet = sheet;
            Sheet.Refresh();
        }

        public AnimatedSprite(Texture texture, int cellWidth, int cellHeight, int fps)
        {
            Sheet = new SpriteSheet(texture, cellWidth, cellHeight);
            Framerate = fps;
            AnimationEnd = Sheet.TotatlCells;
            Sheet.Refresh();
        }

        public void Update(float time)
        {
            if (!Reversed)
            {
                Frame += time * Framerate;
            }
            else
            {
                Frame -= time * Framerate;
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
