using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// From server to client
/// Gives the player his SharedId and Secret
/// </summary>
internal class Acknowledge : IMessage
{
    public int Secret { get; set; }
    public int SharedId { get; set; }

	public MessageType Type => MessageType.Acknowledge;

    public static Acknowledge Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new Acknowledge
        {
            SharedId = d.ReadInt32(),
            Secret = d.ReadInt32(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(SharedId);
        w.Write(Secret);

        return m.ToArray();
	}
}