using SFML.System;
using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Host to client.
/// </summary>
internal class SpawnedProjectile : IMessage
{
	public int OwnerSharedId { get; set; }
	public ushort TypeIndex { get; set; }
	public Vector2f Position { get; set; }
	public Vector2f Speed { get; set; }

	public MessageType Type => MessageType.SpawnedProjectile;

    public static SpawnedProjectile Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new SpawnedProjectile
        {
			OwnerSharedId = d.ReadInt32(),
			TypeIndex = d.ReadUInt16(),
            Position = new Vector2f(d.ReadSingle(), d.ReadSingle()),
            Speed = new Vector2f(d.ReadSingle(), d.ReadSingle()),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
		w.Write(OwnerSharedId);
		w.Write(TypeIndex);
        w.Write(Position.X);
        w.Write(Position.Y);
        w.Write(Speed.X);
        w.Write(Speed.Y);

        return m.ToArray();
	}
}
