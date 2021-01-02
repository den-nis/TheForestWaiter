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
    public class GameObjects : IUpdateDraw
    {
        public Player Player { get; set; }
        public GameObjectContainer<DynamicObject> Enemies { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<DynamicObject> Npcs { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<DynamicObject> Bullets { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<StaticObject> Objects { get; set; } = new GameObjectContainer<StaticObject>();
        public ParticleSystem WorldParticles { get; set; } = new ParticleSystem(GameSettings.Current.MaxWorldParticles);

        public void ClearAll()
        {
            Player = null;
            Enemies.Clear();
            Npcs.Clear();
            Objects.Clear();
        }

        public void CleanUp()
        {
            Objects.CleanupMarkedForDeletion();
            Npcs.CleanupMarkedForDeletion();
            Enemies.CleanupMarkedForDeletion();
            Bullets.CleanupMarkedForDeletion();
        }

        public void Draw(RenderWindow window)
        {
            Objects.Draw(window);
            Npcs.Draw(window);
            Enemies.Draw(window);
            Player.Draw(window);
            Bullets.Draw(window);
            WorldParticles.Draw(window);
        }

        public void Update(float time)
        {
            Objects.Update(time);
            Npcs.Update(time);
            Enemies.Update(time);
            Player.Update(time);
            Bullets.Update(time);
            WorldParticles.Update(time);
        }

        public void LoadAllFromMap(Map map, GameData data)
        {
            var layer = map.Layers.First(l => l.Type == "objectgroup");
            ObjectFactory factory = new ObjectFactory();

            foreach(MapObject inf in layer.Objects)
            {
                StaticObject staticObject = factory.GetStaticObject(inf.Type);
                if (staticObject != null)
                {
                    Objects.Add(staticObject);
                    inf.SetSpawn(staticObject);
                    continue;
                }

                DynamicObject enemy = factory.GetEnemy(inf.Type, data);
                if (enemy != null)
                {
                    Enemies.Add(enemy);
                    inf.SetSpawn(enemy);
                    continue;
                }

                DynamicObject npc = factory.GetNpc(inf.Type, data);
                if (npc != null)
                {
                    Npcs.Add(npc);
                    inf.SetSpawn(npc);
                    continue;
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
