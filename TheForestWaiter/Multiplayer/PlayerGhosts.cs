using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Static;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Multiplayer;

internal class PlayerGhosts
{
    private readonly Dictionary<ushort, Player> _ghosts = new();

    private readonly NetContext _network;
	private readonly NetServer _server;
	private readonly IDebug _debug;

	private readonly ItemRepository _repo;
	private readonly ObjectCreator _creator;
	private readonly GameObjects _objects;
    private readonly Spawner _spawner;

	public PlayerGhosts()
    {
		_debug = IoC.GetInstance<IDebug>();
        _network = IoC.GetInstance<NetContext>();
        _server = IoC.GetInstance<NetServer>();

        _repo = IoC.GetInstance<ItemRepository>();
        _creator = IoC.GetInstance<ObjectCreator>();
        _objects = IoC.GetInstance<GameObjects>();
        _spawner = IoC.GetInstance<Spawner>();
	}

    public Player GetObjectById(ushort playerId)
    {
        return _ghosts[playerId];
    }

    public void AddGhost(ushort playerId)
    {
        var ghost = _creator.CreateAt<Player>(new Vector2f(-10000, -10000)); //TODO: maybe store first position in greetings message?
        ghost.IsGhost = true;
        ghost.PlayerId = playerId;
        _ghosts.Add(playerId, ghost);
        _objects.AddGameObject(ghost);

        _debug.LogNetworking($"Created ghost for player {playerId}");
    }

    /// <param name="relay">Option for the host to sent messages to relay all messages to other clients</param>
	public void HandlePacket(Packet packet, bool relay = false)
	{
        var playerId = BitConverter.ToUInt16(packet.Data.Take(sizeof(ushort)).ToArray());

        if (_network.Settings.IsClient && !_ghosts.ContainsKey(playerId)) //Host creates ghosts when clients connect
        {
            AddGhost(playerId);
        }

        var ghost = _ghosts[playerId];

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
            _network.Traffic.SendToEveryoneExcept(message, playerId);
        }
	}
}