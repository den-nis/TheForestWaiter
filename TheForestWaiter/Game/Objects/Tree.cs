using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Static
{
    internal class Tree : Immovable
    {
        const int FRAME_RATE_MIN = 2;
        const int FRAME_RATE_MAX = 5;

        AnimatedSprite Animation { get; set; }

        public Tree(GameData game, ContentSource content) : base(game)
        {
            Animation = content.Textures.CreateAnimatedSprite("Textures/World/tree.png");
            Size = Animation.Sheet.TileSize.ToVector2f();
            Animation.Framerate = (int)Rng.Range(FRAME_RATE_MIN, FRAME_RATE_MAX);
            Animation.CurrentFrame = (int)Rng.Range(Animation.AnimationStart, Animation.AnimationEnd);
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
