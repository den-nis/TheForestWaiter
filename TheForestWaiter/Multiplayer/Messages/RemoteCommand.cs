using System.IO;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional. For debugginng purposes. Can't run any system commands, only in-game commands.
/// </summary>
internal class RemoteCommand : IMessage
{
    public string Cmd { get; set; }

	public MessageType Type => MessageType.RemoteCommand;

    public static RemoteCommand Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new RemoteCommand
        {
            Cmd = d.ReadString()
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(Cmd);

        return m.ToArray();
	}
}