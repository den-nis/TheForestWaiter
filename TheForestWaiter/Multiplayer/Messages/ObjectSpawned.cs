using SFML.System;
using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Host to client. Server letting client know about a spawned object
/// </summary>
internal class Spawned : IMessage
{   
    public int SharedId { get; set; }
	public ushort TypeIndex { get; set; }
	public Vector2f Position { get; set; }
    public Vector2f Velocity { get; set; }

	public MessageType Type => MessageType.Spawned;

    public static Spawned Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new Spawned
        {
			SharedId = d.ReadInt32(),
			TypeIndex = d.ReadUInt16(),
            Position = new Vector2f(d.ReadSingle(), d.ReadSingle()),
            Velocity = new Vector2f(d.ReadSingle(), d.ReadSingle()),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
		w.Write(SharedId);
		w.Write(TypeIndex);
        w.Write(Position.X);
        w.Write(Position.Y);
        w.Write(Velocity.X);
        w.Write(Velocity.Y);

        return m.ToArray();
	}
}
