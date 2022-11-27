using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

internal class Greetings : IMessage
{
    public string Username { get; set; }

	public MessageType Type => MessageType.Greetings;

    public static Greetings Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new Greetings
        {
            Username = d.ReadString(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(Username);
        
        return m.ToArray();
	}
}
