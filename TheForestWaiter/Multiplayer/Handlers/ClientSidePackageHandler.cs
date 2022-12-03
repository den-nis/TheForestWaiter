using SFML.Graphics;
using System.Net;
using System.Threading;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Essentials;
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
                HandlePlayerPacket(packet);
                break;

            case MessageType.Acknowledge:
				var ack = Acknowledge.Deserialize(packet.Data);
				Network.Settings.MyPlayerId = ack.PlayerId;
				Objects.Player.PlayerId = ack.PlayerId;
				Network.Settings.MySecret = ack.Secret;

				var response = Objects.Player.GenerateInfoMessages(ack.PlayerId);

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

			case MessageType.SpawnProjectile:
				var projectile = SpawnProjectile.Deserialize(packet.Data);

				var owner = projectile.OwnerId == Network.Settings.MyPlayerId ? Objects.Player : Objects.Ghosts.GetById(projectile.OwnerId);

				if (owner == null)
				{
					_debug.LogNetworking($"Could not find projectile for gameobject {projectile.Equals}");
					break;
				}

				var type = Types.GetTypeByIndex(projectile.TypeIndex);
				var obj = _creator.FireProjectile(type, projectile.Position, projectile.Speed, owner);
				Objects.AddGameObject(obj);
				break;
        }
	}
}
