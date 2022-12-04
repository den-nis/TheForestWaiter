using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Particles;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter;

internal class GameObjects
{
	public bool EnableDrawHitBoxes { get; set; } = false;
	
	private readonly Queue<GameObject> _queue = new();
	private readonly UserSettings _settings;
	private readonly ObjectCreator _creator;
	private readonly NetContext _network;
	private readonly Camera _camera;
	private readonly IDebug _debug;

	private int _SharedIdCounter = 0;
	
	public GameObjects()
	{
		_network  = IoC.GetInstance<NetContext>();
		_settings = IoC.GetInstance<UserSettings>();
		_creator  = IoC.GetInstance<ObjectCreator>();
		_camera   = IoC.GetInstance<Camera>();
		_debug    = IoC.GetInstance<IDebug>();

		WorldParticles = new ParticleSystem(_settings.GetInt("Game", "MaxParticles"));
	}

	public Player Player { get; private set; } = null;

	public GameObjectContainer<Immovable> Environment { get; set; } = new();
	public GameObjectContainer<Creature> Creatures { get; set; } = new();
	public GameObjectContainer<Projectile> Projectiles { get; set; } = new();
	public GameObjectContainer<Movable> Other { get; set; } = new();
	public IEnumerable<Player> Players => new[] { this.Player }
		.Concat(Creatures.Where(c => c is Player).Select(p => p as Player));

	public ParticleSystem WorldParticles { get; set; }

	public IEnumerable<Movable> PhysicsObjects =>
		Creatures
			.Concat(Other)
			.Concat(Projectiles);

	public GameObject GetBySharedId(int id)
	{
		return 
			(GameObject)Creatures.GetBySharedId(id) ??
			(GameObject)Projectiles.GetBySharedId(id) ??
			(GameObject)Environment.GetBySharedId(id) ??
			(Player.SharedId == id ? Player : null);
	}

	public bool SharedIdExists(int id) => GetBySharedId(id) != null;	

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
		UpdateRemote();
	}

	public void LoadAllFromMap(Map map, bool onlyClientSideObjects)
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

				if (!onlyClientSideObjects || obj.SpawnOnClient)
				{
					AddGameObject(obj);
				}
			}
			else
			{
				Debug.Fail($"Missing type {inf.Class}");
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

	public Player AddGhostForServer() => AddGhostForClient(-1);

	public Player AddGhostForClient(int sharedId)
	{
		if (!_network.Settings.IsHost && sharedId == -1) throw new InvalidOperationException("Can only use id -1 on server");

		var ghost = _creator.Create<Player>();
		ghost.IsGhost = true;
		ghost.SharedId = _network.Settings.IsClient ? sharedId : ghost.SharedId;
		AddGameObject(ghost);

		return ghost;
	}

	/// <summary>
	/// Adds the object to the correct object container
	/// </summary>
	public void AddGameObject(GameObject obj)
	{
		if (_network.Settings.IsHost)
		{
			obj.SharedId = ++_SharedIdCounter;

			if (!(obj is Player)) 
			{
				_network.Traffic.Send(obj.GetSpawnMessage());
			}
		}

		switch (obj)
		{
			case Player player:

				if (!player.IsGhost)
				{
					Player = player;
				}

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

	private void UpdateRemote()
	{
		if (_network.Settings.IsHost)
		{
			foreach (var obj in Creatures)
			{
				if (obj is Player)
					continue;

				var message = obj.GetUpdateMessage();

				if (message != null)
				{
					_network.Traffic.Send(message);
				}
			}
		}
	}
}
