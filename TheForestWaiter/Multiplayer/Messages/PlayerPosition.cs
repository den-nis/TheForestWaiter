using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class PlayerPosition : IMessage
{
    public ushort PlayerId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

	public MessageType Type => MessageType.PlayerPosition;

    public static PlayerPosition Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new PlayerPosition
        {
            PlayerId = d.ReadUInt16(),
            X = d.ReadSingle(),
            Y = d.ReadSingle(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(PlayerId);
        w.Write(X);
        w.Write(Y);

        return m.ToArray();
	}
}