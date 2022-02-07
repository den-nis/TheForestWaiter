using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Debugging;

namespace TheForestWaiter.Game.Core
{
    abstract class StaticObject : GameObject
    {
        public StaticObject(GameData game) : base(game)
        {
        }
    }
}
