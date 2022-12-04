using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Game.Objects.Abstract
{
	internal abstract class GameObject
	{
		private const float NET_OBJECT_UPDATE_INTERVAL = 0.2f;

		/// <summary>
		/// Id used for referencing shared objects only in multiplayer
		/// </summary>
		public int SharedId { get; set; } = 0;

		public Vector2f Position { get; set; }
		public Vector2f Size { get; set; }
		public Vector2f Center { get => Position + Size / 2; set => Position = value - Size / 2; }

		public IntRect IntRect => new(new Vector2i((int)Position.X, (int)Position.Y), new Vector2i((int)Size.X, (int)Size.Y));
		public FloatRect FloatRect => new(Position, Size);

		private bool _markedForDeletion = false;
		public bool MarkedForDeletion => _markedForDeletion;

		public bool DisableUpdates { get; set; }
		public bool DisableDraws { get; set; }

		public bool IsClientSide { get; protected set; } = false;

		protected GameData Game { get; set; }
		protected NetContext Net { get; set; }

		private Vector2f _lastPosition;

		public GameObject()
		{
			Net = IoC.GetInstance<NetContext>();
			Game = IoC.GetInstance<GameData>();
		}

		public bool Intersects(GameObject other) => FloatRect.Intersects(other.FloatRect);

		public abstract void Draw(RenderWindow window);

		public virtual void Update(float time)
		{
			_lastPosition = Position;
		}

		public virtual void DrawHitbox(RenderWindow window, float lineThickness)
		{
			window.DrawHitBox(Position, Size, Color.Green, lineThickness);
		}

		public void Delete()
		{
			_markedForDeletion = true;
			OnMarkedForDeletion();
		}

		/// <summary>
		/// Can be used for setup logic that needs the tiled object data
		/// </summary>
		public virtual void MapSetup(MapObject mapObject)
		{

		}

		public virtual void OnMarkedForDeletion()
		{
			if (Net.Settings.IsHost)
			{
				Net.Traffic.Send(new MarkedForDeletion
				{
					SharedId = SharedId,
				});
			}
		}

		public IMessage GetSpawnMessage()
		{
			return new Spawned
			{						
				SharedId = SharedId,
				Position = Position,
				Velocity = (this as Movable)?.Velocity ?? default,
				TypeIndex = Types.GetIndexByType(GetType()),
			};
		}

		/// <summary>
		/// Will return null if the last position is the same as the current position
		/// </summary>
		public IMessage GetUpdateMessage()
		{
			return (_lastPosition == Position) ? null : new ObjectUpdate
			{
				SharedId = SharedId,
				Position = Position,
				Velocity = (this as Movable)?.Velocity ?? default,
			};
		}
	}
}
