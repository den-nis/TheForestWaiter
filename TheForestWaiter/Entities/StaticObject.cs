using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Entities
{
    abstract class StaticObject : GameObject
    {
        public StaticObject(GameData game) : base(game)
        {
        }
    }
}
