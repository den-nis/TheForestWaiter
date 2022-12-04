using Newtonsoft.Json;
using SFML.Graphics;
using System.Net;
using System.Threading;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Multiplayer.Handlers;

/// <summary>
/// Reacts to packages coming to the client
/// </summary>
internal class ClientSidePackageHandler : PackageHandler
{
	private readonly IDebug _debug;
	private readonly GameMessages _messages;
	private readonly ObjectCreator _creator;

	public ClientSidePackageHandler()
	{
		//GameObjects / Network is available by parent

		_debug = IoC.GetInstance<IDebug>();
		_messages = IoC.GetInstance<GameMessages>();
		_creator = IoC.GetInstance<ObjectCreator>();
	}

	private void StartReceivingSync(CancellationToken token)
	{
		while(!token.IsCancellationRequested)
        {
            PendingPackets.Enqueue(Network.Traffic.Receive());
        }
	}

	protected override void HandlePacket(Packet packet, EndPoint _)
	{
		switch(packet.Type)
        {
            case MessageType.PlayerPosition:
            case MessageType.PlayerAim:
            case MessageType.PlayerAction:
			case MessageType.PlayerItemAction:
			case MessageType.PlayerShoot:
                HandlePlayerPacket(packet);
                break;

            case MessageType.Acknowledge:
				var ack = Acknowledge.Deserialize(packet.Data);
				Network.Settings.MySharedId = ack.SharedId;
				Network.Settings.MySecret = ack.Secret;
				Objects.Player.SharedId = ack.SharedId;

				var response = Objects.Player.GeneratePlayerMessages();

				foreach (var message in response)
				{
					Network.Traffic.Send(message);
				}

				_messages.PostLocal($"{Color.Yellow.ToColorCode()}Connected to {Network.Settings.ServerEndpoint}!");
				break;

			case MessageType.GameInfo:
				var info = GameInfo.Deserialize(packet.Data); 
				Network.State.WaveNumber = info.WaveNumber;
				break;

			case MessageType.TextMessage:
                var msg = TextMessage.Deserialize(packet.Data);
                _messages.PostLocal(msg.Text);
                break;

			case MessageType.Spawned:
			{
				var obj = Spawned.Deserialize(packet.Data);
				var type = Types.GetTypeByIndex(obj.TypeIndex);

				var instance = _creator.CreateType(type);
				instance.Position = obj.Position;
				instance.SharedId = obj.SharedId;
				if (instance is Movable m) { m.Velocity = obj.Velocity; }

				Objects.AddGameObject(instance);
				break;
			}

			case MessageType.SpawnedProjectile:
			{
				var projectile = SpawnedProjectile.Deserialize(packet.Data);
				var owner = Objects.GetBySharedId(projectile.OwnerSharedId);

				if (owner == null)
				{
					_debug.LogNetworking($"Could not find owner for projectile {projectile}");
					break;
				}

				var type = Types.GetTypeByIndex(projectile.TypeIndex);
				var obj = _creator.FireProjectile(type, projectile.Position, projectile.Speed, owner as Creature);
				Objects.AddGameObject(obj);
				break;
			}

			case MessageType.ObjectUpdate:
			{
				var update = ObjectUpdate.Deserialize(packet.Data);
				var instance = Objects.GetBySharedId(update.SharedId);

				if (instance == null) break;

				instance.Position = update.Position;
				if (instance is Movable mover) mover.Velocity = update.Velocity;
				break;
			}

			case MessageType.Damaged:
			{
				var damageMessage = ObjectDamaged.Deserialize(packet.Data);
				var by = Objects.GetBySharedId(damageMessage.BySharedId);
				var target = Objects.GetBySharedId(damageMessage.ForSharedId);

				if (target != null && target is Creature creature)
				{
					creature.Damage(target as Movable, damageMessage.Damage, damageMessage.Knockback, true);
				}

				break;
			}

			case MessageType.MarkedForDeletion:
			{
				var message = MarkedForDeletion.Deserialize(packet.Data);
				var target = Objects.GetBySharedId(message.SharedId);
				target?.Delete();
				break;
			}

			case MessageType.ObjectKilled:
			{
				var message = ObjectKilled.Deserialize(packet.Data);
				var target = Objects.GetBySharedId(message.SharedId);
				(target as Creature)?.Kill();
				break;
			}
        }
	}
}