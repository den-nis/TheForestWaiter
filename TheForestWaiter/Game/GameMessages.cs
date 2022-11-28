using System;
using System.Collections.Generic;

namespace TheForestWaiter.Game;

internal class GameMessages
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