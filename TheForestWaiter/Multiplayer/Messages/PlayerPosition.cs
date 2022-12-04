using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class PlayerPosition : IMessage
{
    public int SharedId { get; set; }
    public float X { get; set; } //TODO: use vector2f
    public float Y { get; set; }

	public MessageType Type => MessageType.PlayerPosition;

    public static PlayerPosition Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new PlayerPosition
        {
            SharedId = d.ReadInt32(),
            X = d.ReadSingle(),
            Y = d.ReadSingle(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(SharedId);
        w.Write(X);
        w.Write(Y);

        return m.ToArray();
	}
}