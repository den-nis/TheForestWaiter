namespace TheForestWaiter.Multiplayer;

/// <summary>
/// Dependencies related to networking for clientside and serverside.
/// </summary>
internal class NetContext
{
    public NetContext()
    {
        State = IoC.GetInstance<SharedState>();
        Settings = IoC.GetInstance<NetSettings>();
        Traffic = IoC.GetInstance<NetTraffic>();
    }

    public SharedState State { get; set; }
    public NetSettings Settings { get; set; }
    public NetTraffic Traffic { get; set; }
}