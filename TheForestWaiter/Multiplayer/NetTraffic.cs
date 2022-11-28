using SFML.Graphics;
using System;
using System.Net;
using System.Net.Sockets;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Multiplayer;

/// <summary>
/// Gamestate class for basic sending and receiving operations for both client and host.
/// </summary>
internal class NetTraffic : IDisposable
{
    /// <summary>
    /// The socket that is connected to the server
    /// </summary>
    public UdpClient ServerUdp { get; set; }

    /// <summary>
    /// The socket that clients connect to
    /// </summary>
    public UdpClient ClientsUdp { get; set; }

	private readonly NetSettings _settings;
	private readonly IDebug _debug;
	private readonly NetServer _server;
    private readonly GameMessages _messages;

	public NetTraffic()
    {
		_debug = IoC.GetInstance<IDebug>();
		_settings = IoC.GetInstance<NetSettings>();
        _messages = IoC.GetInstance<GameMessages>();

        if (_settings.IsHost)
        {
            _server = IoC.GetInstance<NetServer>();
        }
	}

    public void Setup()
    {
        if (_settings.IsHost)
        {
            ClientsUdp = new UdpClient(_settings.Port);
            _debug.LogNetworking($"Hosted on port : {_settings.Port}");
            _settings.MyPlayerId = 1;
        }
        
        if (_settings.IsClient)
        {
            ServerUdp = new UdpClient();
            ServerUdp.Connect(_settings.ServerEndpoint);
            _debug.LogNetworking($"Target host : {_settings.ServerEndpoint}");

            _messages.PostLocal($"{Color.Yellow.ToColorCode()}Connecting to {_settings.ServerEndpoint}...");
            Send(new Greetings()
			{
				Username = _settings.Username,
			});
        }
    }

    public void PostPublic(string message, bool includeLocal = true)
    {
        if (includeLocal) _messages.PostLocal(message);
        Send(new TextMessage
        {
           Text = message, 
        });
    }

    public void SendToEveryoneExcept(IMessage message, ushort player)
    {
          ValidateMultiplayer();

          var datagram = ToDatagram(message);
          _server.SendToEveryoneExcept(datagram, player, ClientsUdp);
    }

    public void SendTo(IMessage message, ushort player)
    {
        ValidateMultiplayer();

        var datagram = ToDatagram(message);
         _server.SendTo(datagram, player, ClientsUdp);
    }

    public void SendIfMultiplayer(IMessage message)
    {
        if (_settings.IsMultiplayer) Send(message);
    }

    public void Send(IMessage message)
    {
        ValidateMultiplayer();

        var datagram = ToDatagram(message); 
    
        if (_settings.IsHost)
        {
            _server.SendToAll(datagram, ClientsUdp);
        }
        else
        {
            //TODO: Is this really needed?
            if (_settings.ServerEndpoint != null)
            {
                ServerUdp.Client.SendTo(datagram, _settings.ServerEndpoint);
            }
            else
            {
                ServerUdp.Send(datagram);
            }
        }
    }

    private byte[] ToDatagram(IMessage message)
    {
        var type = message.Type;
        var data = message.GetAsBytes();

        var packet = WrapInPacket(type, data);
        return packet.CreateDatagram();
    }

    private Packet WrapInPacket(MessageType type, byte[] data)
    {
        return new Packet
        {
            PlayerId = _settings.MyPlayerId,
            Secret = _settings.MySecret,
            Type = type,
            Data = data,
        };
    }

    public TrackedPacket Receive()
    {
        ValidateMultiplayer();

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _settings.Port);

        while(true)
        {
            var datagram = (_settings.IsHost ? ClientsUdp : ServerUdp).Receive(ref endPoint);
            
            if (datagram.Length == 0)
            {
                _debug.LogNetworking("End of stream. Exiting receive");
                return null;
            }
            
            var packet = Packet.Read(datagram);
        
            if (_settings.IsHost)
            {
                if (!_server.VerifyPacket(packet))
                {
                    _debug.LogNetworking($"Dropping invalid package from {endPoint}");
                    continue;
                }
            }

            return new TrackedPacket(endPoint, packet);
        }
    }

    private void ValidateMultiplayer()
    {
        if (!_settings.IsMultiplayer)
        {
            throw new InvalidOperationException("Attempted to use networking methods in singleplayer");
        }
    }

	public void Dispose()
	{
		ServerUdp?.Dispose();
        ClientsUdp?.Dispose();
        _settings.ResetSessionInfo();
	}
}
