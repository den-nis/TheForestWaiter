using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Host to client.
/// </summary>
internal class MarkedForDeletion : IMessage
{   
    public int SharedId { get; set; }

	public MessageType Type => MessageType.MarkedForDeletion;

    public static MarkedForDeletion Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new MarkedForDeletion
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
