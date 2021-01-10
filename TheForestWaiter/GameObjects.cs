using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheForestWaiter.Objects;
using SFML.Graphics;
using TheForestWaiter.Entites;
using SFML.System;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Environment
{
    class GameObjects : IUpdateDraw
    {
        public Player Player { get; set; }
        public GameObjectContainer<DynamicObject> Enemies { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<DynamicObject> Npcs { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<DynamicObject> Bullets { get; set; } = new GameObjectContainer<DynamicObject>();
        public Chunks Chunks { get; set; } = null;
        public ParticleSystem WorldParticles { get; set; } = new ParticleSystem(GameSettings.Current.MaxWorldParticles);

        public void ClearAll()
        {
            Chunks = null;
            Player = null;
            Enemies.Clear();
            Bullets.Clear();
            Npcs.Clear();
        }

        public void CleanUp()
        {
            Npcs.CleanupMarkedForDeletion();
            Enemies.CleanupMarkedForDeletion();
            Bullets.CleanupMarkedForDeletion();
        }

        public void Draw(RenderWindow window)
        {
            Chunks.Draw(window);
            Npcs.Draw(window);
            Enemies.Draw(window);
            Player.Draw(window);
            Bullets.Draw(window);
            WorldParticles.Draw(window);
        }

        public void Update(float time)
        {
            Chunks.Update(time);
            Npcs.Update(time);
            Enemies.Update(time);
            Player.Update(time);
            Bullets.Update(time);
            WorldParticles.Update(time);
        }

        public void LoadAllFromMap(Map map, World world, GameData data)
        {
            Chunks = new Chunks(world);

            var objects = map.Layers.Where(l => l.Type == "objectgroup").SelectMany(l => l.Objects);
            ObjectFactory factory = new ObjectFactory();

            foreach(MapObject inf in objects)
            {
                StaticObject staticObject = factory.GetStaticObject(inf.Type);
                if (staticObject != null)
                {
                    inf.SetSpawn(staticObject);
                    Chunks.Add(staticObject);
                    continue;
                }
                else
                {
                    Debugging.GameDebug.Log($"Missing StaticObject {inf.Type}");
                }

                if (inf.Type == "Spawn")
                {
                    Player = new Player(data);
                    inf.SetSpawn(Player);
                }
            }
        }
    }
}
