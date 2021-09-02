using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Particles;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Environment;
using SFML.System;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Game
{
    class GameObjects
    {
        private readonly UserSettings _settings;
        private readonly ObjectCreator _creator;
        private readonly IGameDebug _debug;

        public GameObjects(UserSettings settings, ObjectCreator creator, IGameDebug debug)
        {
            _settings = settings;
            _creator = creator;
            _debug = debug;

            WorldParticles = new ParticleSystem(_settings.GetInt("Game", "MaxParticles"));
        }

        public Player Player { get; private set; } = null;
        public Chunks Chunks { get; set; } = null;
        public GameObjectContainer<Creature> Enemies { get; set; } = new GameObjectContainer<Creature>();
        public GameObjectContainer<DynamicObject> Bullets { get; set; } = new GameObjectContainer<DynamicObject>();
        public GameObjectContainer<DynamicObject> Other { get; set; } = new GameObjectContainer<DynamicObject>();
        public ParticleSystem WorldParticles { get; set; }

        public IEnumerable<GameObject> Everything => Enemies
            .Concat(new[] { Player })
            .Concat(Bullets)
            .Concat(Other);

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
            ObjectFactory factory = new(data, _creator); //TODO: DI
            factory.Index();

            //TODO: refactor (no need for static objects in the world anymore)
            foreach(MapObject inf in objects)
            {
                if (inf.Type == "Spawn")
                {
                    Player = _creator.CreateAbove<Player>(inf.Position);
                    continue;
                }

                StaticObject staticObject = factory.GetStaticObject(inf.Type);
                if (staticObject != null)
                {
                    staticObject.Position = new Vector2f(inf.Position.X, inf.Position.Y - staticObject.Size.Y);
                    staticObject.PrepareSpawn(inf);
                    Chunks.Add(staticObject);
                    continue;
                }
                else
                {
                    _debug.Log($"Missing StaticObject {inf.Type}");
                }
            }
        }
    }
}
