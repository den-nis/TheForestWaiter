using SFML.Graphics;
using System.Net;
using System.Threading;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Multiplayer.Handlers;

/// <summary>
/// Reacts to packages coming to the client
/// </summary>
internal class ClientSidePackageHandler : PackageHandler
{
	private readonly NetworkSettings _network;
	private readonly SharedState _state;
	private readonly PlayerGhosts _ghosts;
	private readonly GameObjects _game;
	private readonly NetworkTraffic _traffic;
	private readonly GameMessages _messages;

	public ClientSidePackageHandler()
	{
		_network = IoC.GetInstance<NetworkSettings>();
		_state = IoC.GetInstance<SharedState>();
		_ghosts = IoC.GetInstance<PlayerGhosts>();
		_game = IoC.GetInstance<GameObjects>();
		_traffic = IoC.GetInstance<NetworkTraffic>();
		_messages = IoC.GetInstance<GameMessages>();
	}

	private void StartReceivingSync(CancellationToken token)
	{
		while(!token.IsCancellationRequested)
        {
            PendingPackets.Enqueue(Traffic.Receive());
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
                _ghosts.HandlePacket(packet);
                break;

            case MessageType.Acknowledge:
				var ack = Acknowledge.Deserialize(packet.Data);
				_network.MyPlayerId = ack.PlayerId;
				_network.MySecret = ack.Secret;

				var response = _game.Player.GenerateInfoMessages(ack.PlayerId);

				foreach (var message in response)
				{
					_traffic.Send(message);
				}

				_messages.PostLocal($"{Color.Yellow.ToColorCode()}Connected to {_network.ServerEndpoint}!");
				break;

			case MessageType.GameInfo:
				var info = GameInfo.Deserialize(packet.Data); 
				_state.WaveNumber = info.WaveNumber;
				break;

			case MessageType.TextMessage:
                var msg = TextMessage.Deserialize(packet.Data);
                _messages.PostLocal(msg.Text);
                break;
        }
	}
}
