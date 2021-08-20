﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Entities;
using TheForestWaiter.Environment;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Objects.Static
{
    class Grass : StaticObject
    {
        const int FRAME_RATE_MIN = 2;
        const int FRAME_RATE_MAX = 5;

        AnimatedSprite Animation { get; set; }

        public Grass(GameData game) : base(game)
        {
            Animation = GameContent.Textures.CreateAnimatedSprite("Textures\\World\\grass.png");
            Animation.Framerate = (int)Rng.Range(FRAME_RATE_MIN, FRAME_RATE_MAX);
            Animation.CurrentFrame = (int)Rng.Range(Animation.AnimationStart, Animation.AnimationEnd);
            Size = Animation.Sheet.TileSize.ToVector2f();
        }

        public override void Update(float time)
        {
            Animation.Sheet.Sprite.Position = Position;
            Animation.Update(time);
        }

		public override void Draw(RenderWindow window)
        { 
            window.Draw(Animation);
        }
    }
}