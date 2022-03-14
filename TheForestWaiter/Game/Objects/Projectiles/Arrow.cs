using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
    internal class Arrow : Projectile
    {
        public Arrow(GameData game, ContentSource content) : base(game, content)
        {
            Drag = new Vector2f(30, 0);
            Gravity = 300;
            RotationSpeed = 2f;
            ExplosionParticleName = null; //TODO: add particle

            SetTexture("Textures/Projectiles/arrow.png");

        }
    }
}
