using SFML.Graphics;
using System;

namespace TheForestWaiter.States
{
	internal class StateTransition
	{
		public Type TargetState { get; set; }
		public float Length { get; set; } = 1;
		public Color Color { get; set; } = Color.Black;
	}
}
