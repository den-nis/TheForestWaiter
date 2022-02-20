using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Items
{
	internal class Apple : Pickup
	{
		private const int HEALING_POINTS = 10;

		public Apple(GameData game, ContentSource content) :
			base("Textures/Items/apple.png", game, content)
		{

		}

		protected override void OnPickup(Creature player)
		{
			player.Heal(HEALING_POINTS);
		}
	}
}
