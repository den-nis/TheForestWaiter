using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Weapons.Bullets;
using TheForestWaiter.Game.Particles;

namespace TheForestWaiter.Game
{
    internal class GameObjects
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

        public GameObjectContainer<StaticObject> Environment { get; set; } = new();
        public GameObjectContainer<Creature> Actors { get; set; } = new();
        public GameObjectContainer<PhysicsObject> Bullets { get; set; } = new();
        public GameObjectContainer<PhysicsObject> Other { get; set; } = new();

        public ParticleSystem WorldParticles { get; set; }

        public IEnumerable<PhysicsObject> PhysicsObjects => Actors
            .Concat(Actors)
            .Concat(Bullets)
            .Concat(Other);

        private IEnumerable<IGameObjectContainer> GetAllContainers()
        {
            yield return Environment;
            yield return Other;
            yield return Actors;
            yield return Bullets;
        }

        private void ForAllContainers(Action<IGameObjectContainer> func)
        {
            foreach (var container in GetAllContainers())
            {
                func(container);
            }
        }

        public IEnumerable<Creature> Creatures => Actors.Concat(new[] { Player });

        public void ClearAll()
        {
            Player = null;
            ForAllContainers(c => c.Clear());
        }

        public void CleanUp()
        {
            ForAllContainers(c => c.CleanupMarkedForDeletion());
        }

        public void Draw(RenderWindow window)
        {
            ForAllContainers(c => c.Draw(window));
            WorldParticles.Draw(window);

            if (EnableDrawHitBoxes) 
            {
                DrawHitBoxes(window);
            }
        }

        public void Update(float time)
        {
            ForAllContainers(c => c.Update(time));
            WorldParticles.Update(time);
        }

        public void LoadAllFromMap(Map map, World world, GameData data)
        {
            Player = null;

            var objects = map.Layers.Where(l => l.Type == "objectgroup").SelectMany(l => l.Objects);
            foreach (MapObject inf in objects)
            {
                Types.GameObjects.TryGetValue(inf.Type, out Type type);
                if (type != null)
                {
                    var obj = _creator.CreateType(type);
                    obj.Position = new Vector2f(inf.X, inf.Y - obj.Size.Y);
                    obj.MapSetup(inf);
                    AddGameObject(obj);
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
        public void AddGameObject(GameObject obj)
        {
            switch (obj)
            {
                case Player player:
                    Player = player;
                    Actors.Add(player);
                    break;

                case Bullet bullet:      Bullets.Add(bullet); break;
                case Creature enemy:     Actors.Add(enemy); break;
                case PhysicsObject pObj: Other.Add(pObj); break;
                case StaticObject sObj:  Environment.Add(sObj); break;

                default:
                    throw new KeyNotFoundException($"No container found for \"{obj.GetType().Name}\"");
            }
        }
    }
}
