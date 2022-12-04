using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class PlayerItems : IMessage
{
    public int SharedId { get; set; }
    public int[] Items { get; set; }
    public int EquipedIndex { get; set; }

	public MessageType Type => MessageType.PlayerItemAction;

    public static PlayerItems Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);

        var id = d.ReadInt32();
        var size = d.ReadInt32();
        int[] items = new int[size];
        
        for (int i = 0; i < size; i++)
        {
            items[i] = d.ReadInt32();
        }
        
        return new PlayerItems 
        { 
            SharedId = id,
            Items = items,
            EquipedIndex = d.ReadInt32(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);

        w.Write(SharedId);
        w.Write(Items.Length);
    
        foreach (var item in Items)
        {
            w.Write(item);
        }

        w.Write(EquipedIndex);

        return m.ToArray();
	}
}