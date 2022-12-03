using System;
using System.Collections.Generic;
using TheForestWaiter.Multiplayer;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Game;

internal class GameMessages
{
    public event Action<string> OnLocalMessage;

	private LocalGameMessages _messages;
	private NetContext _networking;

	public GameMessages()
    {
        _messages = IoC.GetInstance<LocalGameMessages>();
        _networking = IoC.GetInstance<NetContext>();

        _messages.OnMessage += (m) => OnLocalMessage?.Invoke(m);
    }

    public void PostLocal(string message) => _messages.PostLocal(message);

    public void Post(string message, bool includeLocal = true)
    {
        if (includeLocal) _messages.PostLocal(message);

        if (_networking.Settings.IsMultiplayer)
        {
            _networking.Traffic.Send(new TextMessage
            {
                Text = message, 
            });
        }
    }
}

internal class LocalGameMessages
{
    private const int MAX_HISTORY = 4000;

	public event Action<string> OnMessage;
    public IEnumerable<string> Lines => _buffer;

    private Queue<string> _buffer = new();

    /// <summary>
    /// (NOT threadsafe) send a message locally
    /// </summary>
    public void PostLocal(string message)
    {
        _buffer.Enqueue(message);
        OnMessage?.Invoke(message);

        if (_buffer.Count > MAX_HISTORY)
        {
            _buffer.Dequeue();
        }
    }
}