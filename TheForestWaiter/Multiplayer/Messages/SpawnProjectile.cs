using SFML.System;
using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Client to host. Player wants to fire a projectile
/// </summary>
internal class SpawnProjectile : IMessage
{
	public ushort OwnerId { get; set; }
	public ushort TypeIndex { get; set; }
	public Vector2f Position { get; set; }
	public Vector2f Speed { get; set; }

	public MessageType Type => MessageType.SpawnProjectile;

    public static SpawnProjectile Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new SpawnProjectile
        {
			OwnerId = d.ReadUInt16(),
			TypeIndex = d.ReadUInt16(),
            Position = new Vector2f(d.ReadSingle(), d.ReadSingle()),
            Speed = new Vector2f(d.ReadSingle(), d.ReadSingle()),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
		w.Write(OwnerId);
		w.Write(TypeIndex);
        w.Write(Position.X);
        w.Write(Position.Y);
        w.Write(Speed.X);
        w.Write(Speed.Y);

        return m.ToArray();
	}
}
