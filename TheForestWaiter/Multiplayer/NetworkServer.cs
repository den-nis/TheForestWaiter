using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Multiplayer;

namespace TheForestWaiter;

/// <summary>
/// Gamestate class. Manages server specific operations and data.
/// </summary>
internal class NetworkServer
{
    public int Players => _clients.Count;
    public IReadOnlyList<Client> Clients => _clients;

    private List<Client> _clients = new();
    private Dictionary<int, Client> _clientsBySecret = new();
    private Dictionary<ushort, Client> _clientsById = new();

	private readonly NetworkSettings _network;
	private readonly IDebug _debug;

	public NetworkServer()
    {
		_network = IoC.GetInstance<NetworkSettings>();
		_debug = IoC.GetInstance<IDebug>();
	}

	public void SendToEveryoneExcept(byte[] datagram, ushort player, UdpClient udpClient)
	{
		foreach (var client in _clients)
        {
            if (player != client.PlayerId)
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

    public void SendTo(byte[] data, ushort playerId, UdpClient udpClient)
    {
        var client = GetClientById(playerId);

        if (client != null)
        {
            udpClient.Client.SendTo(data, client.EndPoint);
        }
    }

    /// <summary>
    /// Returns the added client
    /// </summary>
    public Client AddClient(EndPoint endpoint, string username)
    {
        var client = new Client
        {
            Secret = Rng.RangeInt(int.MinValue, int.MaxValue),
            PlayerId = GetId(),
            EndPoint = endpoint,
            LastMessage = DateTime.Now,
            Username = username,
        };

        AddClient(client);
        return client;
    }

    private ushort GetId()
    {
        return (ushort)Enumerable.Range(2, short.MaxValue)
            .First(x => GetClientById((ushort)x) == null);
    }

    private void AddClient(Client client)
    {
        _clients.Add(client);
        _clientsById.Add(client.PlayerId, client);
        _clientsBySecret.Add(client.Secret, client);
    }

    public Client GetClientBySecret(int secret)
    {
        if (_clientsBySecret.TryGetValue(secret, out Client client)) return client;
        return null;
    }

    public Client GetClientById(ushort id)
    {
        if (_clientsById.TryGetValue(id, out Client client)) return client;
        return null;
    }

    public bool VerifyPacket(Packet packet)
    {
        return (GetClientBySecret(packet.Secret)?.PlayerId ?? 0) == packet.PlayerId;
    }
}
