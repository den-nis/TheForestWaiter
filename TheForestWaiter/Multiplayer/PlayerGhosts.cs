using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Game;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Objects;

namespace TheForestWaiter.Multiplayer;

internal class PlayerGhosts : GameObjectContainer<Player>
{
    private readonly Dictionary<ushort, Player> _ghostsByPlayerId = new();
	private readonly ObjectCreator _creator;

	public PlayerGhosts()
    {
        _creator = IoC.GetInstance<ObjectCreator>();
	}

    public Player GetById(ushort playerId)
    {
        _ghostsByPlayerId.TryGetValue(playerId, out Player player);
        return player;
    }

    public void CreateAndAddGhost(ushort playerId)
    {
        var ghost = _creator.CreateAt<Player>(new Vector2f(-10000, -10000)); //TODO: maybe store first position in greetings message?
        ghost.IsGhost = true;
        ghost.PlayerId = playerId;

        _ghostsByPlayerId.Add(playerId, ghost);
        this.Add(ghost);
    }
}