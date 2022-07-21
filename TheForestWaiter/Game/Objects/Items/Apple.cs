using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class Apple : Pickup
	{
		private const int HEALING_POINTS = 10;

		public Apple() : base("Textures/Items/apple.png")
		{

		}

		protected override void OnPickup(Creature player)
		{
			player.Heal(HEALING_POINTS);
		}
	}
}
