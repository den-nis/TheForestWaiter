using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TheForestWaiter.Objects;
using SFML.Graphics;
using TheForestWaiter.Entites;
using SFML.System;
using TheForestWaiter.Particles;
using TheForestWaiter.Objects.Enemies;

namespace TheForestWaiter.Environment
{
    class GameObjects : IUpdateDraw
    {
        public Player Player { get; set; } = null;
        public Chunks Chunks { get; set; } = null;
        public GameObjectContainer<Creature> Enemies { get; set; } = new GameObjectContainer<Creature>();
        public GameObjectContainer<DynamicObject> Bullets { get; set; } = new GameObjectContainer<DynamicObject>();
        public ParticleSystem WorldParticles { get; set; } = new ParticleSystem(GameSettings.Current.MaxWorldParticles);

        public IEnumerable<Creature> Creatures => Enemies.Concat(new[] { Player });

        public void ClearAll()
        {
            Chunks = null;
            Player = null;
            Enemies.Clear();
            Bullets.Clear();
            WorldParticles.Clear();
        }

        public void CleanUp()
        {
            Enemies.CleanupMarkedForDeletion();
            Bullets.CleanupMarkedForDeletion();
        }

        public void Draw(RenderWindow window)
        {
            Chunks.Draw(window);
            Enemies.Draw(window);
            Player.Draw(window);
            Bullets.Draw(window);
            WorldParticles.Draw(window);
        }

        public void Update(float time)
        {
            Chunks.Update(time);
            Enemies.Update(time);
            Player.Update(time);
            Bullets.Update(time);
            WorldParticles.Update(time);

            Chunks.LoadChunksAt(Player.Center);
        }

        public void LoadAllFromMap(Map map, World world, GameData data)
        {
            Player = null;
            Chunks = new Chunks(world);

            var objects = map.Layers.Where(l => l.Type == "objectgroup").SelectMany(l => l.Objects);
            ObjectFactory factory = new ObjectFactory(data);

            foreach(MapObject inf in objects)
            {
                if (inf.Type == "Spawn")
                {
                    Player = new Player(data);
                    inf.SetSpawn(Player);
                    continue;
                }

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
            }
        }
    }
}
