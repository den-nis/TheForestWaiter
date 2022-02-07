using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Particles;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Environment;
using SFML.System;
using TheForestWaiter.Game.Debugging;
using System;
using TheForestWaiter.Game.Objects.Weapons.Bullets;

namespace TheForestWaiter.Game
{
    class GameObjects
    {
        public bool EnableDrawHitBoxes { get; set; } = false;

        private readonly UserSettings _settings;
        private readonly ObjectCreator _creator;
        private readonly Camera _camera;
        private readonly IDebug _debug;

        public GameObjects(UserSettings settings, ObjectCreator creator, Camera camera, IDebug debug)
        {
            _settings = settings;
            _creator = creator;
            _camera = camera;
            _debug = debug;

            WorldParticles = new ParticleSystem(_settings.GetInt("Game", "MaxParticles"));
        }

        public Player Player { get; private set; } = null;
        public Chunks Chunks { get; set; } = null;
        public GameObjectContainer<Creature> Enemies { get; set; } = new GameObjectContainer<Creature>();
        public GameObjectContainer<PhysicsObject> Bullets { get; set; } = new GameObjectContainer<PhysicsObject>();
        public GameObjectContainer<PhysicsObject> Other { get; set; } = new GameObjectContainer<PhysicsObject>();
        public ParticleSystem WorldParticles { get; set; }

        public IEnumerable<PhysicsObject> PhysicsObjects => Enemies
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

            if (EnableDrawHitBoxes) 
            {
                DrawHitBoxes(window);
            }
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
            foreach (MapObject inf in objects)
            {
                Types.GameObjects.TryGetValue(inf.Type, out Type type);
                if (type != null)
                {
                    var obj = _creator.CreateType(type);
                    obj.Position = new Vector2f(inf.X, inf.Y - obj.Size.Y);
                    obj.MapSetup(inf);
                    AddAuto(obj);
                }
                else
                {
                    _debug.Log($"Missing type {inf.Type}");
                }
            }
        }

        public void DrawHitBoxes(RenderWindow window)
        {
            foreach(var obj in PhysicsObjects)
            {
                obj.DrawHitbox(window, _camera.Scale);
            }
        }

        /// <summary>
        /// Adds the object to the correct object container
        /// </summary>
        public void AddAuto(GameObject obj)
        {
            switch (obj)
            {
                case Player player:
                    Player = player;
                    break;

                case Bullet bullet:
                    Bullets.Add(bullet);
                    break;

                case Creature enemy:
                    Enemies.Add(enemy);
                    break;

                case PhysicsObject pObj:
                    Other.Add(pObj);
                    break;

                case StaticObject sObj:
                    Chunks.GetChunkAt(obj.Center).Objects.Add(sObj);
                    break;

                default:
                    throw new KeyNotFoundException($"No container found for \"{obj.GetType().Name}\"");
            }
        }
    }
}
