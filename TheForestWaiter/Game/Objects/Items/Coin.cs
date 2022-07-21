using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class Coin : Pickup
	{
		public Coin() : base("Textures/Items/coin.png")
		{

		}

		protected override void OnPickup(Creature player)
		{
			Game.Session.Coins++;
		}
	}
}
