using SFML.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects;
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
                Network.Traffic.SendToEveryoneExcept(msg, packet.SharedId);
                _messages.PostLocal(msg.Text);
                break;

            case MessageType.RemoteCommand:
                var command = RemoteCommand.Deserialize(packet.Data);
                _debug.InjectCommand(command.Cmd);
                break;
        }
	}

    private void HandleGreetings(Packet packet, EndPoint endpoint)
    {
        var greet = Greetings.Deserialize(packet.Data);
        var player = Objects.AddGhostForServer();
        var client = _server.AddClient(endpoint, greet.Username, player.SharedId);
    
        Network.Traffic.SendTo(new Acknowledge
        {
            Secret = client.Secret,
            SharedId = player.SharedId,
        }, player.SharedId);

        SendGameInfo(client.SharedId);

        _messages.Post($"{Color.Green.ToColorCode()}{client.Username} joined the game!");
    }

    private void SendGameInfo(int sharedId)
    {
        List<IMessage> messages = Objects.Player.GenerateInfoMessages().ToList();

        foreach (var player in _server.Clients)
        {
            if (player.SharedId != sharedId)
            {
                var instance = Objects.Ghosts.GetBySharedId(player.SharedId);

                if (instance.Alive)
                {
                    messages.AddRange(instance.GenerateInfoMessages());
                }
            }
        }

        foreach (var creature in Objects.Creatures.Where(x => x.Alive && !(x is Player)) )
        {
            messages.Add(creature.GetSpawnMessage());
        }

        foreach (var message in messages)
        {
            Network.Traffic.SendTo(message, sharedId);
        }

        Network.Traffic.SendTo(new GameInfo
        {
            WaveNumber = _spawner.CurrentWave
        }, sharedId);
    }
}
