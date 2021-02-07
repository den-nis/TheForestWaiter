using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entites;
using TheForestWaiter.Essentials;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Objects.Static
{
    class Tree : StaticObject
    {
        const int FRAME_RATE_MIN = 2;
        const int FRAME_RATE_MAX = 5;

        AnimatedSprite Animation { get; set; }

        public Tree(GameData game) : base(game)
        {
            Animation = GameContent.Textures.CreateAnimatedSprite("Textures\\World\\tree.png");
            Size = Animation.Sheet.TileSize.ToVector2f();
            Animation.Framerate = (int)Rng.Range(FRAME_RATE_MIN, FRAME_RATE_MAX);
            Animation.CurrentFrame = (int)Rng.Range(Animation.AnimationStart, Animation.AnimationEnd);
        }

        public override void Update(float time)
        {
            Animation.Sheet.Sprite.Position = Position;
            Animation.Update(time);
            base.Update(time);
        }

        public override void Draw(RenderWindow window)
        {
            window.Draw(Animation);
        }
    }
}
