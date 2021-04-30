using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;
using static SFML.Window.Mouse;

namespace TheForestWaiter
{
	static class UserSettings
	{
		private const string SETTINGS_PATH = "Settings.ini";
		private static IniData _data;

		public static Key Left { get; private set; }
		public static Key Right { get; private set; }
		public static Key Jump { get; private set; }
		public static Key FullScreen { get; private set; }
		public static Button Shoot { get; private set; }

		public static void Load()
		{
			FileIniDataParser parser = new();
			_data = parser.ReadFile(SETTINGS_PATH);

			Left = Enum.Parse<Key>(Get("Controls", "Left"));
			Right = Enum.Parse<Key>(Get("Controls", "Right"));
			Jump = Enum.Parse<Key>(Get("Controls", "Jump"));
			FullScreen = Enum.Parse<Key>(Get("Controls", "FullScreen"));
			Shoot = Enum.Parse<Button>(Get("Controls", "Shoot"));
		}

		public static string Get(string category, string name)
		{
			return _data[category][name];
		}

		public static float GetFloat(string category, string name)
		{
			return float.Parse(_data[category][name]);
		}

		public static int GetInt(string category, string name)
		{
			return int.Parse(_data[category][name]);
		}

		public static bool GetBool(string category, string name)
		{
			return bool.Parse(_data[category][name]);
		}
	}
}
