using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class PlayerAim : IMessage
{
    public ushort PlayerId { get; set; }
    public float Angle { get; set; }

	public MessageType Type => MessageType.PlayerAim;

    public static PlayerAim Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new PlayerAim 
        { 
            PlayerId = d.ReadUInt16(),
            Angle = d.ReadSingle(), 
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);

        w.Write(PlayerId);
        w.Write(Angle);

        return m.ToArray();
	}
}
