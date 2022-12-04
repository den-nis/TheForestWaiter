using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class PlayerAim : IMessage
{
    public int SharedId { get; set; }
    public float Angle { get; set; }

	public MessageType Type => MessageType.PlayerAim;

    public static PlayerAim Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new PlayerAim 
        { 
            SharedId = d.ReadInt32(),
            Angle = d.ReadSingle(), 
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);

        w.Write(SharedId);
        w.Write(Angle);

        return m.ToArray();
	}
}
