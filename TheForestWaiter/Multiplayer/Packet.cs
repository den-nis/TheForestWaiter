using System.IO;
using System.Net;

namespace TheForestWaiter.Multiplayer;

internal class Packet
{
    public ushort PlayerId { get; set; }
    public int Secret { get; set; }

    public MessageType Type { get; set; }
    public byte[] Data { get; set; }

    public bool IsAnonymous => PlayerId == 0;

    public byte[] CreateDatagram()
    {
        using MemoryStream ms = new();
        using BinaryWriter wr = new(ms);

        wr.Write(Secret);
        wr.Write(PlayerId);
        wr.Write((short)Type);
        wr.Write(Data);
        wr.Flush();

        return ms.ToArray();
    }

    /// <summary>
    /// Generate a packet object from a raw datagram
    /// <summary/>
    public static Packet Read(byte[] datagram)
    {
        using MemoryStream ms = new(datagram);
        using BinaryReader br = new(ms);

        var secret = br.ReadInt32();
        var player = br.ReadUInt16();
        var type = (MessageType)br.ReadInt16();
        var data = br.ReadBytes(datagram.Length - (int)ms.Position);

        return new Packet
        {
            Type = type,
            Data = data,
            PlayerId = player,
            Secret = secret,
        };
    }
}

/// <summary>
/// Packet that also includes origin
/// </summary>
internal class TrackedPacket
{
    public IPEndPoint Endpoint { get; set; } 
    public Packet Source { get; set; }

    public TrackedPacket(IPEndPoint origin, Packet source)
    {
        Endpoint = origin;
        Source = source;
    }
}
