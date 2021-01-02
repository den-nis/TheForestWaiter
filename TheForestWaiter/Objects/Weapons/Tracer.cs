using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Extensions;

namespace TheForestWaiter.Objects.Weapons
{
    public class Tracer
    {
        public Color Color { get; set; }
        public Vector2f Start { get; set; }
        public float Distance { get; set; }
        public float Rotation { get; set; }

        public float Life { get; set; }
        public float RemainingLife { get; set; }

        public void Draw(RenderWindow window)
        {
            Vertex[] vertexes =
            {
                new Vertex(Start + new Vector2f(0, 0.5f).RotateBy(Rotation), Color),
                new Vertex(Start + new Vector2f(0, -0.5f).RotateBy(Rotation), Color),
                new Vertex(Start + new Vector2f(Distance, -0.5f).RotateBy(Rotation), Color),
                new Vertex(Start + new Vector2f(Distance, 0.5f).RotateBy(Rotation), Color),
            };

            window.Draw(vertexes, PrimitiveType.Quads);
        }
    }
}
