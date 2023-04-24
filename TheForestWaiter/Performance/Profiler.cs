using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game;

namespace TheForestWaiter.Performance;

internal class Profiler
{
	//TODO: have a solution for linux
	private const string FONT_NAME_LINUX = @"/usr/share/fonts/truetype/freefont/FreeMono.ttf";
	private const string FONT_NAME_WINDOWS = @"C:\Windows\Fonts\consola.ttf";

	private const int WIDTH = 1100;
	private const int HEIGHT = 350;

	private const float BAR_START_X = WIDTH/2 - 200;
	private const float BAR_THICKNESS = 10;
	private const float BAR_RIGHT_MARGIN = 20;

	private readonly Font font;  

	private class Tracker
	{
		private const int NUMBER_OF_SAMPLES = 150;
		private const float TRACKER_OFFSET = 15;
		private const uint CHARACTER_SIZE = 15;

		public double Average { get; set; } = double.NaN;

		private readonly Color _color;
		private readonly Text _text;
		private readonly RectangleShape _bar;

		private readonly ProfileCategory _name;

		public Tracker(Font font, Color color, ProfileCategory name)
		{
			_bar = new RectangleShape();
			_text = new Text(string.Empty, font);
			_color = color;
			_name = name;
		}

		public void PushTime(double time)
		{
			Average = double.IsNaN(Average) ? time : Average;
			Average -= Average / NUMBER_OF_SAMPLES;
			Average += time / NUMBER_OF_SAMPLES;
		}

		public void Draw(RenderWindow window, int position, float percentage)
		{
			string time = Math.Round(Average, 2).ToString("0.00 μs").PadLeft(8);
			string name = _name.ToString().PadRight(30, '_');

			_text.DisplayedString = $"{name} {time}";
			_text.Position = new Vector2f(0, position * TRACKER_OFFSET);
			_text.FillColor = _color;
			_text.CharacterSize = CHARACTER_SIZE;

			_bar.FillColor = _color;
			_bar.Position = new Vector2f(BAR_START_X, position * TRACKER_OFFSET + BAR_THICKNESS/2);
			_bar.Size = new Vector2f((WIDTH - BAR_START_X - BAR_RIGHT_MARGIN) * percentage, BAR_THICKNESS);

			window.Draw(_bar);
			window.Draw(_text);
		}

		public void Reset()
		{
			Average = double.NaN;
		}
	}

	private readonly Dictionary<ProfileCategory, Tracker> _trackers = new();
	private readonly ConcurrentQueue<ProfilerPacket[]> _source;

	public Profiler(ConcurrentQueue<ProfilerPacket[]> source)
	{
		_source = source;

		if (OperatingSystem.IsWindows())
		{
			font = new(FONT_NAME_WINDOWS);
		} 
		else
		{
			font = new(FONT_NAME_LINUX);
		}
	}

	public void Start()
	{
		RenderWindow window = new(new VideoMode(WIDTH, HEIGHT), "Profiler", Styles.Close);

		window.SetFramerateLimit(60);
		window.Closed += (_, _) => window.Close();
		window.KeyPressed += WindowKeyPressed;

		while (window.IsOpen)
		{
			window.DispatchEvents();

			while (_source.TryDequeue(out var packet))
			{
				AddPackets(packet);
			}

			window.Clear(Color.Black);
			Draw(window);
			window.Display();
		}
	}

	private void WindowKeyPressed(object sender, KeyEventArgs e)
	{
		if (e.Code == Keyboard.Key.R)
		{
			foreach (var tracker in _trackers.Values)
			{
				tracker.Reset();
			}
		}
	}

	private void Draw(RenderWindow window)
	{
		var max = _trackers.Any() ? _trackers.Values.Max(t => t.Average) : 1;
		int position = 0;

		foreach (var tracker in _trackers.Values)
		{
			tracker.Draw(window, position, (float)tracker.Average / (float)max);
			position++;
		}
	}

	private void AddPackets(ProfilerPacket[] packets)
	{
		foreach (var packet in packets)
		{
			if (!_trackers.TryGetValue(packet.Category, out Tracker tracker))
			{
				tracker = _trackers[packet.Category] = new Tracker(font, RandomColor(), packet.Category);
			}

			tracker.PushTime(packet.Microseconds);
		}
	}

	private static Color RandomColor() => new(
			(byte)Rng.RangeInt(100, 255),
			(byte)Rng.RangeInt(100, 255),
			(byte)Rng.RangeInt(100, 255));
}
