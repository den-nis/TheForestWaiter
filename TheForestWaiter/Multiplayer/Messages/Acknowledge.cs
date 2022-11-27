using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// From server to client
/// Gives the player his PlayerId and Secret
/// </summary>
internal class Acknowledge : IMessage
{
    public int Secret { get; set; }
    public ushort PlayerId { get; set; }

	public MessageType Type => MessageType.Acknowledge;

    public static Acknowledge Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new Acknowledge
        {
            PlayerId = d.ReadUInt16(),
            Secret = d.ReadInt32(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(PlayerId);
        w.Write(Secret);

        return m.ToArray();
	}
}