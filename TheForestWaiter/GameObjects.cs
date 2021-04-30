﻿using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Objects;
using SFML.Graphics;
using TheForestWaiter.Entities;
using TheForestWaiter.Particles;

namespace TheForestWaiter.Environment
{
	class GameObjects
    {
        public Player Player { get; set; } = null;
        public Chunks Chunks { get; set; } = null;
        public GameObjectContainer<Creature> Enemies { get; set; } = new GameObjectContainer<Creature>();
        public GameObjectContainer<DynamicObject> Bullets { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<DynamicObject> Other { get; set; } = new GameObjectContainer<DynamicObject>();
        public ParticleSystem WorldParticles { get; set; } = new ParticleSystem(UserSettings.GetInt("Game", "MaxParticles"));

        public IEnumerable<Creature> Creatures => Enemies.Concat(new[] { Player });

        public void ClearAll()
        {
            Chunks = null;
            Player = null;
            Enemies.Clear();
            Other.Clear();
            Bullets.Clear();
            WorldParticles.Clear();
        }

        public void CleanUp()
        {
            Enemies.CleanupMarkedForDeletion();
            Bullets.CleanupMarkedForDeletion();
            Other.CleanupMarkedForDeletion();
        }

        public void Draw(RenderWindow window)
        {
            Chunks.Draw(window);
            Other.Draw(window);
            Enemies.Draw(window);
            Player.Draw(window);
            Bullets.Draw(window);
            WorldParticles.Draw(window);
        }

        public void Update(float time)
        {
            Chunks.Update(time);
            Other.Update(time);
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
            ObjectFactory factory = new(data);
            factory.Index();

            foreach(MapObject inf in objects)
            {
                if (inf.Type == "Spawn")
                {
                    Player = new Player(data);
                    Player.SetSpawn(inf.Position);
                    continue;
                }

                StaticObject staticObject = factory.GetStaticObject(inf.Type);
                if (staticObject != null)
                {
                    staticObject.SetSpawn(inf.Position);
                    staticObject.PrepareSpawn(inf);
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
