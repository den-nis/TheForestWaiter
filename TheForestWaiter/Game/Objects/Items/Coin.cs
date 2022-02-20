using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class Coin : Pickup
	{
		public Coin(GameData game, ContentSource content) :
			base("Textures/Items/coin.png", game, content)
		{

		}

		protected override void OnPickup(Creature player)
		{
			Game.Session.Coins++;
		}
	}
}
