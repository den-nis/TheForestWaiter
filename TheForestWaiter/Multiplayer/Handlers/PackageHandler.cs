using SFML.System;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Multiplayer.Handlers;

internal abstract class PackageHandler : IDisposable
{
    private Task _job;
	private IDebug _debug;
    private CancellationTokenSource _cts = new();

    protected GameObjects Objects { get; set; }
	protected NetContext Network { get; set; }
	protected ConcurrentQueue<TrackedPacket> PendingPackets { get; set; } = new();

    public PackageHandler()
    {
        _debug = IoC.GetInstance<IDebug>();
        Network = IoC.GetInstance<NetContext>();
        Objects = IoC.GetInstance<GameObjects>();
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
                var receive = Task.Run(() => Network.Traffic.Receive());

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

    /// <param name="relay">Option for the host to sent messages to relay all messages to other clients</param>
    protected void HandlePlayerPacket(Packet packet, bool relay = false)
    {
        var playerId = BitConverter.ToUInt16(packet.Data.Take(sizeof(ushort)).ToArray());

        if (Network.Settings.IsClient && Objects.Ghosts.GetById(playerId) == null) //Host creates ghosts when clients connect
        {
            Objects.Ghosts.CreateAndAddGhost(playerId);
        }

        var ghost = Objects.Ghosts.GetById(playerId);

        IMessage message = null;
		switch (packet.Type)
        {
            case MessageType.PlayerPosition:
                var position = PlayerPosition.Deserialize(packet.Data);
                ghost.Position = new Vector2f(position.X, position.Y);
                message = position;
                break;

            case MessageType.PlayerAim:
                var aim = PlayerAim.Deserialize(packet.Data);
                ghost.Controller.Aim(aim.Angle);
                message = aim;
                break;

            case MessageType.PlayerItemAction:
                var itemInfo = PlayerItems.Deserialize(packet.Data);
                ghost.Inventory.Overwrite(itemInfo.Items);
                ghost.Inventory.Select(itemInfo.EquipedIndex);
                message = itemInfo;
                break;

            case MessageType.PlayerAction:
                var act = PlayerAction.Deserialize(packet.Data);
                ghost.Controller.Toggle(act.Action, act.State);
                message = act;
                break;
        }

        if (relay)
        {
            Network.Traffic.SendToEveryoneExcept(message, playerId);
        }
    }

	public void Dispose()
	{
        _cts?.Cancel();
        _job?.Wait();
	}
}