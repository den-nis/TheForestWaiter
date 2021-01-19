using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entites;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Objects.Static
{
    class Tree : StaticObject
    {
        AnimatedSprite Sprite { get; set; } 

        public Tree(GameData game) : base(game)
        {
            //Sprite = new AnimatedSprite(new SpriteSheet(GameContent.GetTexture()))
        }

        public override void Update(float time)
        {
            base.Update(time);
        }

        public override void Draw(RenderWindow window)
        {
            
        }
    }
}
