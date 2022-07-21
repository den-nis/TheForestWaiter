using System;

namespace TheForestWaiter.Game.Constants
{
	[Flags]
	internal enum WorldCollisionFlags
	{
		None = 0x0,
		Horizontal = 0x1,
		Vertical = 0x2,
		Top = 0x4,
		Bottom = 0x8,
		Left = 0x10,
		Right = 0x20,
	}
}
