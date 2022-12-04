using System;
using System.Net;

namespace TheForestWaiter.Multiplayer;

internal record Client
{
    public int Secret { get; set; }
    public int SharedId { get; set; }
    public EndPoint EndPoint { get; set; }
    public DateTime LastMessage { get; set; }
    public string Username { get; set; }
}