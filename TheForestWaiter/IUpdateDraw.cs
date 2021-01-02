using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter
{
    interface IUpdateDraw
    {
        public void Draw(RenderWindow window);
        public void Update(float time);
    }
}
