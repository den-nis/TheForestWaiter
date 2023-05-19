using IniParser;
using IniParser.Model;
using System;
using static SFML.Window.Keyboard;
using static SFML.Window.Mouse;

namespace TheForestWaiter
{
	internal class UserSettings : ISetup
	{
		private const string SETTINGS_PATH = "Settings.ini";
		private IniData _data;

		public Key Left { get; private set; }
		public Key Right { get; private set; }
		public Key Jump { get; private set; }
		public Key ToggleShop { get; private set; }
		public Key ShowWeaponWheel { get; private set; }
		public Key FullScreen { get; private set; }
		public Button Primary { get; private set; }

		public void Setup()
		{
			FileIniDataParser parser = new();
			_data = parser.ReadFile(SETTINGS_PATH);

			Left = Enum.Parse<Key>(Get("Controls", "Left"));
			Right = Enum.Parse<Key>(Get("Controls", "Right"));
			Jump = Enum.Parse<Key>(Get("Controls", "Jump"));
			ToggleShop = Enum.Parse<Key>(Get("Controls", "ToggleShop"));
			ShowWeaponWheel = Enum.Parse<Key>(Get("Controls", "ShowWeaponWheel"));
			FullScreen = Enum.Parse<Key>(Get("Controls", "FullScreen"));
			Primary = Enum.Parse<Button>(Get("Controls", "Shoot"));
		}

		public string Get(string category, string name)
		{
			return _data[category][name];
		}

		public float GetFloat(string category, string name)
		{
			return float.Parse(_data[category][name]);
		}

		public int GetInt(string category, string name)
		{
			return int.Parse(_data[category][name]);
		}

		public bool GetBool(string category, string name)
		{
			return bool.Parse(_data[category][name]);
		}
	}
}
