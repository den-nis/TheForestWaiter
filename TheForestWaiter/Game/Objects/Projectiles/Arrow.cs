using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Projectiles
{
    internal class Arrow : Projectile
    {
        public Arrow(GameData game, ContentSource content) : base(game, content)
        {
            Damage = 15;
            Penetration = 5;
            Gravity = 300;
            RotationSpeed = 2f;
            ExplosionParticleName = "Particles/spark_gray.particle";
            Penetration = 5;
            RemoveOnLowMovement = false;

            SetTexture("Textures/Projectiles/arrow.png");
        }
    }
}
