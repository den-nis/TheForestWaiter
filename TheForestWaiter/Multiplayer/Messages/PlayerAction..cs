using System.IO;
using TheForestWaiter.Game.Constants;

namespace TheForestWaiter.Multiplayer.Messages;

/// <summary>
/// Bidirectional
/// </summary>
internal class PlayerAction : IMessage
{
    public int SharedId { get; set; }
    public ActionTypes Action { get; set; }
    public bool State { get; set; }

	public MessageType Type => MessageType.PlayerAction;

    public static PlayerAction Deserialize(byte[] data)
    {
        using MemoryStream m = new(data);
        using BinaryReader d = new(m);
        
        return new PlayerAction
        {
            SharedId = d.ReadInt32(),
            Action = (ActionTypes)d.ReadUInt16(),
            State = d.ReadBoolean(),
        };
    }

	public byte[] GetAsBytes()
	{
        using MemoryStream m = new();
        using BinaryWriter w = new(m);
        
        w.Write(SharedId);
        w.Write((ushort)Action);
        w.Write(State);

        return m.ToArray();
	}
}