using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

internal class ObjectKilled : IMessage
{   
    public int SharedId { get; set; }

	public MessageType Type => MessageType.ObjectKilled;

    public static ObjectKilled Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new ObjectKilled
        {
			SharedId = d.ReadInt32(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
		w.Write(SharedId);

        return m.ToArray();
	}
}
