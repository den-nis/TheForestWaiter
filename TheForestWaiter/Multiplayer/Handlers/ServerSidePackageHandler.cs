using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Static;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Multiplayer.Handlers;

/// <summary>
/// Reacts to packages coming to the server
/// </summary>
internal class ServerSidePackageHandler : PackageHandler
{
	private readonly IDebug _debug;
    private readonly Spawner _spawner;
	private readonly ObjectCreator _creator;
	private readonly GameMessages _messages;
    private readonly NetServer _server;

	public ServerSidePackageHandler()
    {
		_debug = IoC.GetInstance<IDebug>();
        _spawner = IoC.GetInstance<Spawner>();
        _creator = IoC.GetInstance<ObjectCreator>();
        _messages = IoC.GetInstance<GameMessages>();
        _server = IoC.GetInstance<NetServer>();;

	}

	protected override void HandlePacket(Packet packet, EndPoint endpoint)
	{
        if (packet.IsAnonymous)
        {
            if (packet.Type == MessageType.Greetings)
                HandleGreetings(packet, endpoint);

            return; //Other types require ack
        }

		switch (packet.Type)
        {
            case MessageType.PlayerPosition:
            case MessageType.PlayerAim:
            case MessageType.PlayerAction:
            case MessageType.PlayerItemAction:
                HandlePlayerPacket(packet, true);
                break;

            case MessageType.TextMessage:
                var msg = TextMessage.Deserialize(packet.Data);
                Network.Traffic.SendToEveryoneExcept(msg, packet.PlayerId);
                _messages.PostLocal(msg.Text);
                break;
        }
	}

    private void HandleGreetings(Packet packet, EndPoint endpoint)
    {
        //TODO: rate limiting?
        var greet = Greetings.Deserialize(packet.Data);
        var client = _server.AddClient(endpoint, greet.Username);
    
        Network.Traffic.SendTo(new Acknowledge
        {
            PlayerId = client.PlayerId,
            Secret = client.Secret,
        }, client.PlayerId);

        Objects.Ghosts.CreateAndAddGhost(client.PlayerId);

        SendGameInfo(client.PlayerId);

        _messages.Post($"{Color.Green.ToColorCode()}{client.Username} joined the game!");
    }

    private void SendGameInfo(ushort playerId)
    {
        List<IMessage> messages = Objects.Player.GenerateInfoMessages(1).ToList();

        foreach (var player in _server.Clients)
        {
            if (player.PlayerId != playerId)
            {
                messages.AddRange(Objects.Ghosts.GetById(player.PlayerId).GenerateInfoMessages(player.PlayerId));
            }
        }

        foreach (var message in messages)
        {
            Network.Traffic.SendTo(message, playerId);
        }

        Network.Traffic.SendTo(new GameInfo
        {
            WaveNumber = _spawner.CurrentWave
        }, playerId);
    }
}
