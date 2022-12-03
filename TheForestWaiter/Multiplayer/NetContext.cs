namespace TheForestWaiter.Multiplayer;

/// <summary>
/// Dependencies related to networking for clientside and serverside.
/// </summary>
internal class NetContext
{
    public NetContext()
    {
        Settings = IoC.GetInstance<NetSettings>();

        if (Settings.IsMultiplayer)
        {
            Traffic = IoC.GetInstance<NetTraffic>();
            State = IoC.GetInstance<SharedState>();
        }
    }

    public SharedState State { get; set; }
    public NetSettings Settings { get; set; }
    public NetTraffic Traffic { get; set; }
}