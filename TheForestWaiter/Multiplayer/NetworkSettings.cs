using System.Net;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter;

/// <summary>
/// Class that stores and manages network data for the client and host.
/// This class should be available at any time, even outside the gamestate.
/// </summary>
internal class NetworkSettings
{
    public IPEndPoint ServerEndpoint { get; set; }

	public short Port { get; private set;}
    public bool IsClient { get; private set; } = false;
    public bool IsHost { get; private set; } = false;
    public bool IsMultiplayer => IsClient || IsHost;

    public ushort MyPlayerId { get; set; } = 0;
    public int MySecret { get; set; } = 0;

    public NetworkSettings(UserSettings settings, IDebug debug)
    {
        var ip = IPAddress.Parse(settings.Get("Multiplayer", "Server"));
        var port = (short)settings.GetInt("Multiplayer", "Port");
        
        switch(settings.Get("Multiplayer", "Mode"))
        {
            case "Client":
                Setup(isHost: false, ip, port);
                break;

            case "Host":
                Setup(isHost: true, ip, port);
                break;
        }
	}

    public void Setup(bool isHost, IPAddress ip, short port)
    {   
        MyPlayerId = 0;
        MySecret = 0;
        IsClient = !isHost;
        IsHost = isHost;
        Port = port;
        ServerEndpoint = new IPEndPoint(ip, port);
    }
}
