﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class Apple : Pickup
	{
		private const int HEALING_POINTS = 10;

		public Apple(GameData game, ContentSource content) :
		base("Textures\\Items\\apple.png", game, content)
		{

		}

		protected override void OnPickup(Player player)
		{
			player.Heal(HEALING_POINTS);
		}
	}
}
