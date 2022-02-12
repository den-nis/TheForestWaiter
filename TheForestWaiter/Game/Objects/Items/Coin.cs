using SFML.Graphics;
using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Game.Objects.Items;

namespace TheForestWaiter.Game.Gibs
{
	internal class Coin : Pickup
	{
		public Coin(GameData game, ContentSource content) : 
			base("Textures\\Items\\coin.png", game, content)
		{

		}

		protected override void OnPickup(Player player)
		{
			Game.Session.Coins++;
		}
	}
}
