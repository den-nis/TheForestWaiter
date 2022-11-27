using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheForestWaiter.Content;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Game.Hud.Sections;

internal class MessagesHud : HudSection
{
    private const int MAX_ON_SCREEN = 10;
    private const int MESSAGE_LIFE = 45;
    private const float MESSAGE_SCALE = 0.3f; 

    private class Msg
    {
        public float Life { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
    }

    private readonly List<Msg> _messages = new();
    private readonly SpriteFont _text;
    private readonly GameData _game;
    private readonly GameMessages _source;

    public MessagesHud(float scale) : base(scale)
    {
        var content = IoC.GetInstance<ContentSource>();
        _game = IoC.GetInstance<GameData>();
        _source = IoC.GetInstance<GameMessages>();

        _text = new SpriteFont(content.Textures.CreateSpriteSheet("Textures/Hud/generic_text.png"));

        //TODO: set Size?

        _source.OnMessage += OnMessageHandler;
    }

    public void OnMessageHandler(string message)
    {
        (var text, Color color) = ExtractColorPrefix(message);

        _messages.Add(new Msg
        {
            Life = MESSAGE_LIFE,
            Text = text,
            Color = color,
        });

        if (_messages.Count > MAX_ON_SCREEN)
        {
            _messages.RemoveAt(0);
        }
    }

    public override void Draw(RenderWindow window)
    {
        for (int i = 0; i < _messages.Count; i++)
        {
            var msg = _messages[i];

            var position = _messages.Count - i - 1;
            var offset = _text.Sheet.TileSize.Y * position * Scale * MESSAGE_SCALE;

            _text.Position = GetPosition(window) - new Vector2f(0, offset);
            _text.Scale = Scale * MESSAGE_SCALE;

            var a = (byte)((msg.Life / MESSAGE_LIFE) * byte.MaxValue);

            _text.IndexOffset = ' ';
            _text.Color = new Color(msg.Color.R, msg.Color.G, msg.Color.B, a);
            _text.SetText(msg.Text);
            _text.Draw(window);
        }
    }

    public override void Update(float time)
    {
        foreach (var message in _messages)
        {
            message.Life -= time;
        }

        while (_messages.Count > 0 && _messages[0].Life < 0)
        {
            _messages.RemoveAt(0);
        }
    }

    private (string, Color) ExtractColorPrefix(string text)
    {
        var pattern = "^(\\[[0-9]+,[0-9]+,[0-9]+\\])+";
        
        Regex regex = new Regex(pattern);
        var matches = regex.Matches(text);

        if (matches.Any())
        {
            var match = Regex.Matches(matches[0].Value, "\\[[0-9]+,[0-9]+,[0-9]+\\]")[0];
            var parts = match.Value.Trim('[', ']').Split(',');
            
            byte r = byte.Parse(parts[0]),
                g = byte.Parse(parts[1]),
                b = byte.Parse(parts[2]);

            var length = matches.Sum(x => x.Length);
            return (text[length..], new Color(r,g,b));
        }

        return (text, Color.White);
    }

    public override bool IsMouseOnAnyButton() => false;
    public override void OnMouseMove(Vector2i mouse) { }
    public override void OnPrimaryReleased() { }
    public override void OnPrimaryPressed() { }
}
