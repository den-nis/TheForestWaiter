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
    private readonly NetworkServer _server;
    private readonly NetworkTraffic _traffic;
    private readonly Spawner _spawner;
	private readonly ObjectCreator _creator;
	private readonly GameObjects _objects;
    private readonly PlayerGhosts _ghosts;
	private readonly GameData _game;
	private readonly GameMessages _messages;

	public ServerSidePackageHandler()
    {
		_debug = IoC.GetInstance<IDebug>();
        _server = IoC.GetInstance<NetworkServer>();
        _traffic = IoC.GetInstance<NetworkTraffic>();
        _spawner = IoC.GetInstance<Spawner>();
        _creator = IoC.GetInstance<ObjectCreator>();
        _objects = IoC.GetInstance<GameObjects>();
        _ghosts = IoC.GetInstance<PlayerGhosts>();
        _game = IoC.GetInstance<GameData>();
        _messages = IoC.GetInstance<GameMessages>();
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
                _ghosts.HandlePacket(packet, true);
                break;

            case MessageType.TextMessage:
                var msg = TextMessage.Deserialize(packet.Data);
                _traffic.SendToEveryoneExcept(msg, packet.PlayerId);
                _messages.PostLocal(msg.Text);
                break;
        }
	}

    private void HandleGreetings(Packet packet, EndPoint endpoint)
    {
        //TODO: rate limiting?
        var greet = Greetings.Deserialize(packet.Data);
        var client = _server.AddClient(endpoint, greet.Username);
    
        _traffic.SendTo(new Acknowledge
        {
            PlayerId = client.PlayerId,
            Secret = client.Secret,
        }, client.PlayerId);

        _ghosts.AddGhost(client.PlayerId);

        SendGameInfo(client.PlayerId);

        _messages.PostPublic($"{Color.Green.ToTfwColorCode()}{client.Username} joined the game!");
    }


    private void SendGameInfo(ushort playerId)
    {
        List<IMessage> messages = _game.Objects.Player.GenerateInfoMessages(1).ToList();

        foreach (var player in _server.Clients)
        {
            if (player.PlayerId != playerId)
            {
                messages.AddRange(_ghosts.GetObjectById(player.PlayerId).GenerateInfoMessages(player.PlayerId));
            }
        }

        foreach (var message in messages)
        {
            _traffic.SendTo(message, playerId);
        }

        _traffic.SendTo(new GameInfo
        {
            WaveNumber = _spawner.CurrentWave
        }, playerId);
    }
}
