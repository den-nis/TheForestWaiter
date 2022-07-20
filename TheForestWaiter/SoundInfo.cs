namespace TheForestWaiter
{
	internal record SoundInfo
	{
		public string Identifier { get; }
		public float Volume { get; set; } = 50;
		public float Pitch { get; set; } = 1;

		public float PitchVariation { get; set; }
		public float VolumeVariation { get; set; }

		private bool _none = false;

		public static SoundInfo None
		{
			get
			{
				var sound = new SoundInfo(string.Empty);
				sound._none = true;
				return sound;
			}
		}

		public bool IsSilent() => _none;

		public SoundInfo(string identifier)
		{
			Identifier = identifier;
		}
	}
}
