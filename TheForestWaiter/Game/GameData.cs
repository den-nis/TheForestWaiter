using System;
using System.Collections.Generic;
using System.Text;
using TheForestWaiter.Environment;
using TheForestWaiter.Particles;

namespace TheForestWaiter
{
    class GameData
    {
        public World World { get; private set; }
        public GameObjects Objects { get; private set; } = new GameObjects();

        public void LoadFromMap(Map map, IProgress<string> progress = null)
        {
            progress?.Report("Loading map...");
            World = World.LoadFromMap(map);

            progress?.Report("Loading objects...");
            Objects.ClearAll();
            Objects.LoadAllFromMap(map, World, this);
        }
    }
}
