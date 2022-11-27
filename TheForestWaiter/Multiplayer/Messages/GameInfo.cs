using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Message from server to client. 
/// </summary>
internal class GameInfo : IMessage
{
    public int WaveNumber { get; set; }

	public MessageType Type => MessageType.GameInfo;

    public static GameInfo Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new GameInfo
        {
            WaveNumber = d.ReadInt32(),
        };
        
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(WaveNumber);

        return m.ToArray();
	}
}