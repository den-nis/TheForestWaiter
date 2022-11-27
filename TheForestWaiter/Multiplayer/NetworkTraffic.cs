using System;
using System.Net;
using System.Net.Sockets;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter;

/// <summary>
/// Gamestate class for basic sending and receiving operations for both client and host.
/// </summary>
internal class NetworkTraffic : IDisposable
{
    /// <summary>
    /// The socket that is connected to the server
    /// </summary>
    public UdpClient ServerUdp { get; set; }

    /// <summary>
    /// The socket that clients connect to
    /// </summary>
    public UdpClient ClientsUdp { get; set; }

    public ushort MyId => _network.MyPlayerId;
    public bool IsClient => _network.IsClient;
    public bool IsHost => _network.IsHost;
    public bool IsMultiplayer => _network.IsMultiplayer;

	private readonly NetworkSettings _network;
	private readonly IDebug _debug;
	private readonly NetworkServer _server;

	public NetworkTraffic()
    {
		_network = IoC.GetInstance<NetworkSettings>();
		_debug = IoC.GetInstance<IDebug>();

        if (_network.IsHost)
        {
            _server = IoC.GetInstance<NetworkServer>();
        }
	}

    public void Setup()
    {
        if (IsHost)
        {
            ClientsUdp = new UdpClient(_network.Port);
            _debug.LogNetworking($"Hosted on port : {_network.Port}");
            _network.MyPlayerId = 1;
        }
        
        if (IsClient)
        {
            ServerUdp = new UdpClient();
            ServerUdp.Connect(_network.ServerEndpoint);
            _debug.LogNetworking($"Target host : {_network.ServerEndpoint}");
        }
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
        if (IsMultiplayer) Send(message);
    }

    public void Send(IMessage message)
    {
        ValidateMultiplayer();

        var datagram = ToDatagram(message); 
    
        if (IsHost)
        {
            _server.SendToAll(datagram, ClientsUdp);
        }
        else
        {
            //TODO: Is this really needed?
            if (_network.ServerEndpoint != null)
            {
                ServerUdp.Client.SendTo(datagram, _network.ServerEndpoint);
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
            PlayerId = _network.MyPlayerId,
            Secret = _network.MySecret,
            Type = type,
            Data = data,
        };
    }

    public TrackedPacket Receive()
    {
        ValidateMultiplayer();

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _network.Port);

        while(true)
        {
            var datagram = (IsHost ? ClientsUdp : ServerUdp).Receive(ref endPoint);
            
            if (datagram.Length == 0)
            {
                _debug.LogNetworking("End of stream. Exiting receive");
                return null;
            }
            
            var packet = Packet.Read(datagram);
        
            if (IsHost)
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
        if (!_network.IsMultiplayer)
        {
            throw new InvalidOperationException("Attempted to use networking methods in singleplayer");
        }
    }

	public void Dispose()
	{
		ServerUdp?.Dispose();
        ClientsUdp?.Dispose();
	}
}
