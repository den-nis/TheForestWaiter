using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Entites;

namespace TheForestWaiter.Environment
{
    public class MapObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public string Type { get; set; }
        public bool Visible { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public void SetSpawn(GameObject obj)
        {
            obj.Position = new Vector2f(X - obj.Size.X / 2, Y - obj.Size.Y);
        }
    }
}
