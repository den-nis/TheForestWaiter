using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TheForestWaiter.Game.Debugging;

namespace TheForestWaiter.Multiplayer.Handlers;

internal abstract class PackageHandler : IDisposable
{
    private Task _job;
	private IDebug _debug;
    private CancellationTokenSource _cts = new();

	protected NetworkSettings Network { get; set; }
	protected NetworkTraffic Traffic { get; set; }
	protected ConcurrentQueue<TrackedPacket> PendingPackets { get; set; } = new();

    public PackageHandler()
    {
        _debug = IoC.GetInstance<IDebug>();
        Network = IoC.GetInstance<NetworkSettings>();
        Traffic = IoC.GetInstance<NetworkTraffic>();
    }

    public void HandleIncoming()
    {
        while (!PendingPackets.IsEmpty)
        {
            if (PendingPackets.TryDequeue(out TrackedPacket packet))
            {
                HandlePacket(packet.Source, packet.Endpoint);
            }
        }
    }

    protected abstract void HandlePacket(Packet packet, EndPoint endpoint);

    public void StartReceiving()
    {
        _job = Task.Factory.StartNew(() => ReceivingJob(_cts.Token), _cts.Token);
    }

    private void ReceivingJob(CancellationToken token)
    {
        var tcs = new TaskCompletionSource<bool>();
        token.Register(c => tcs.TrySetResult(true), null);

        while (!token.IsCancellationRequested)
        {
            try
            {
                var receive = Task.Run(() => Traffic.Receive());

                Task.WaitAny(receive, tcs.Task);

                if (receive.IsCompleted)
                {
                    var packet = receive.Result;
                    if (packet != null) PendingPackets.Enqueue(packet);
                }
                else if (tcs.Task.IsCompleted)
                {
                    break;
                }
            }
            catch(Exception e)
            {
                _debug.LogNetworking("Something went wrong while receiving message");
                _debug.LogNetworking(e.ToString());
            }
        }
    }

	public void Dispose()
	{
        _cts?.Cancel();
        _job?.Wait();
	}
}