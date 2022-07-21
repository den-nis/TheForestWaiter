using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Particles;

namespace TheForestWaiter
{
	internal class GameObjects
	{
		public bool EnableDrawHitBoxes { get; set; } = false;

		private readonly Queue<GameObject> _queue = new();
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

		public GameObjectContainer<Immovable> Environment { get; set; } = new();
		public GameObjectContainer<Creature> Creatures { get; set; } = new();
		public GameObjectContainer<Projectile> Projectiles { get; set; } = new();
		public GameObjectContainer<Movable> Other { get; set; } = new();

		public ParticleSystem WorldParticles { get; set; }

		public IEnumerable<Movable> PhysicsObjects =>
			 Creatures
			.Concat(Other)
			.Concat(Projectiles);

		private IEnumerable<IGameObjectContainer> GetAllContainers()
		{
			yield return Environment;
			yield return Other;
			yield return Creatures;
			yield return Projectiles;
		}

		private void ForAllContainers(Action<IGameObjectContainer> func)
		{
			foreach (var container in GetAllContainers())
			{
				func(container);
			}
		}

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
			HandleQueue();
			ForAllContainers(c => c.Update(time));
			WorldParticles.Update(time);
		}

		public void LoadAllFromMap(Map map)
		{
			Player = null;

			var objects = map.Layers.Where(l => l.Type == "objectgroup").SelectMany(l => l.Objects);
			foreach (MapObject inf in objects)
			{
				Types.GameObjects.TryGetValue(inf.Class, out Type type);
				if (type != null)
				{
					var obj = _creator.CreateType(type);
					obj.Position = new Vector2f(inf.X, inf.Y - obj.Size.Y);
					obj.MapSetup(inf);
					AddGameObject(obj);
				}
				else
				{
					_debug.Log($"Missing type {inf.Class}");
				}
			}
		}

		public void DrawHitBoxes(RenderWindow window)
		{
			foreach (var obj in PhysicsObjects)
			{
				obj.DrawHitbox(window, _camera.Scale);
			}
		}

		private void HandleQueue()
		{
			while (_queue.Count > 0)
			{
				AddGameObject(_queue.Dequeue());
			}
		}

		/// <summary>
		/// Will add the gameobjects next frame
		/// </summary>
		public void QueueAddGameObject(GameObject obj) => _queue.Enqueue(obj);

		/// <summary>
		/// Adds the object to the correct object container
		/// </summary>
		public void AddGameObject(GameObject obj)
		{
			switch (obj)
			{
				case Player player:
					Player = player;
					Creatures.Add(player);
					break;

				case SmallBullet bullet: Projectiles.Add(bullet); break;
				case Creature enemy: Creatures.Add(enemy); break;
				case Movable pObj: Other.Add(pObj); break;
				case Immovable sObj: Environment.Add(sObj); break;

				default:
					throw new KeyNotFoundException($"No container found for \"{obj.GetType().Name}\"");
			}
		}
	}
}
