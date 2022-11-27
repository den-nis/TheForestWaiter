using System;
using System.Collections.Generic;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Game;

internal class GameMessages
{
    private const int MAX_HISTORY = 4000;
	private readonly NetworkTraffic _traffic;

	public event Action<string> OnMessage;
    public IEnumerable<string> Lines => _buffer;

    private Queue<string> _buffer = new();

    public GameMessages(NetworkTraffic traffic)
    {
	    _traffic = traffic;
	}

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

    /// <summary>
    /// (NOT threadsafe) send a message locally and to other players (on multiplayer)
    /// </summary>
    public void PostPublic(string message)
    {
        PostLocal(message);

        _traffic.Send(new TextMessage
        {
           Text = message, 
        });
    }
}