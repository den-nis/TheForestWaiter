using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Host to client. Server letting client know about a damaged creature
/// </summary>
internal class ObjectDamaged : IMessage
{   
    public int BySharedId { get; set; }
    public int ForSharedId { get; set; }
    public float Damage { get; set; }
	public float Knockback { get; set; }

	public MessageType Type => MessageType.Damaged;

    public static ObjectDamaged Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new ObjectDamaged
        {
			BySharedId = d.ReadInt32(),
            ForSharedId = d.ReadInt32(),
			Damage = d.ReadSingle(),
            Knockback = d.ReadSingle(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
		w.Write(BySharedId);
		w.Write(ForSharedId);
		w.Write(Damage);
        w.Write(Knockback);

        return m.ToArray();
	}
}
