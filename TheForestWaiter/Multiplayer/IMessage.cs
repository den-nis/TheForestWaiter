namespace TheForestWaiter.Multiplayer;

internal interface IMessage
{
    public MessageType Type { get; }
    
    public byte[] GetAsBytes();
}