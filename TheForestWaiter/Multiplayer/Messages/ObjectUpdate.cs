using SFML.System;
using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Host to client. Server letting client know about a moved object
/// </summary>
internal class ObjectUpdate : IMessage
{   
    public int SharedId { get; set; }
	public Vector2f Position { get; set; }
    public Vector2f Velocity { get; set; }

	public MessageType Type => MessageType.ObjectUpdate;

    public static ObjectUpdate Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new ObjectUpdate
        {
			SharedId = d.ReadInt32(),
            Position = new Vector2f(d.ReadSingle(), d.ReadSingle()),
            Velocity = new Vector2f(d.ReadSingle(), d.ReadSingle()),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
		w.Write(SharedId);
        w.Write(Position.X);
        w.Write(Position.Y);
        w.Write(Velocity.X);
        w.Write(Velocity.Y);

        return m.ToArray();
	}
}
