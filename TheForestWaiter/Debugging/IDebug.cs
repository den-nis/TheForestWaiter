﻿using SFML.Graphics;
using System;

namespace TheForestWaiter.Game.Debugging
{
	interface IDebug : IDisposable
	{
		void Setup();

		void Update(float time);

		void Draw(RenderWindow window);
	}
}