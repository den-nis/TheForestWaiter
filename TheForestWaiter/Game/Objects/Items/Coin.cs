using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class Coin : Pickup
	{
		public Coin(GameData game, ContentSource content) :
			base("Textures\\Items\\coin.png", game, content)
		{

		}

		protected override void OnPickup(Creature player)
		{
			Game.Session.Coins++;
		}
	}
}
