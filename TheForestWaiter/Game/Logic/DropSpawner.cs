using SFML.System;
using TheForestWaiter.Game.Gibs;
using TheForestWaiter.Game.Objects.Items;

namespace TheForestWaiter.Game.Logic
{
	/// <summary>
	/// Spawns all the things enemies spawn when they die
	/// </summary>
	internal class DropSpawner
	{
		public float ChanceOfHeartDrop { get; set; } = 0.1f;
		public int MinAmountCoins { get; set; } = 0;
		public int MaxAmountCoins { get; set; } = 4;

		private readonly PickupSpawner _pickupSpawner;
		private readonly GibSpawner _gibSpawner;

		public DropSpawner(PickupSpawner pickupSpawner, GibSpawner gibSpawner)
		{
			_pickupSpawner = pickupSpawner;
			_gibSpawner = gibSpawner;
		}

		public void Setup(string gibSheetName)
		{
			_gibSpawner.Setup(gibSheetName);
		}

		public void Spawn(Vector2f at)
		{
			_gibSpawner.SpawnAll(at);
			_pickupSpawner.SpawnAmount(at, Rng.RangeInt(MinAmountCoins, MaxAmountCoins), ChanceOfHeartDrop > Rng.Float() ? 1 : 0);
		}
	}
}
