using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class TextMessage : IMessage
{
    public string Text { get; set; }

	public MessageType Type => MessageType.TextMessage;

    public static TextMessage Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new TextMessage
        {
            Text = d.ReadString()
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(Text);

        return m.ToArray();
	}
}