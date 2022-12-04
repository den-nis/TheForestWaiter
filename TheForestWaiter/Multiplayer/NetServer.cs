using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Multiplayer;

/// <summary>
/// Gamestate class. Manages server specific operations and data.
/// </summary>
internal class NetServer
{
    public int Players => _clients.Count;
    public IReadOnlyList<Client> Clients => _clients;

    private List<Client> _clients = new();
    private Dictionary<int, Client> _clientsBySecret = new();
    private Dictionary<int, Client> _clientsBySharedId = new();

	private readonly NetSettings _network;
	private readonly IDebug _debug;

	public NetServer()
    {
		_network = IoC.GetInstance<NetSettings>();
		_debug = IoC.GetInstance<IDebug>();
	}

	public void SendToEveryoneExcept(byte[] datagram, int sharedId, UdpClient udpClient)
	{
		foreach (var client in _clients)
        {
            if (sharedId != client.SharedId)
            {
                udpClient.Client.SendTo(datagram, client.EndPoint);
            }
        }
	}

    public void SendToAll(byte[] datagram, UdpClient udpClient)
    {
        foreach (var client in _clients)
        {
            udpClient.Client.SendTo(datagram, client.EndPoint);
        }
    }

    public void SendTo(byte[] data, int sharedId, UdpClient udpClient)
    {
        var client = GetClientById(sharedId);

        if (client != null)
        {
            udpClient.Client.SendTo(data, client.EndPoint);
        }
    }

    /// <summary>
    /// Returns the added client
    /// </summary>
    public Client AddClient(EndPoint endpoint, string username, int sharedId)
    {
        var client = new Client
        {
            Secret = Rng.RangeInt(int.MinValue, int.MaxValue),
            SharedId = sharedId,
            EndPoint = endpoint,
            LastMessage = DateTime.Now,
            Username = username,
        };

        AddClient(client);
        return client;
    }

    private void AddClient(Client client)
    {
        _clients.Add(client);
        _clientsBySharedId.Add(client.SharedId, client);
        _clientsBySecret.Add(client.Secret, client);
    }

    public Client GetClientBySecret(int secret)
    {
        if (_clientsBySecret.TryGetValue(secret, out Client client)) return client;
        return null;
    }

    public Client GetClientById(int id)
    {
        if (_clientsBySharedId.TryGetValue(id, out Client client)) return client;
        return null;
    }

    public bool VerifyPacket(Packet packet)
    {
        return (GetClientBySecret(packet.Secret)?.SharedId ?? 0) == packet.SharedId;
    }
}
