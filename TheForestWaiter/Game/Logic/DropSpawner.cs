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

		public PickupSpawner PickupSpawner { get; private set; }
		public GibSpawner GibSpawner { get; private set; }

		public DropSpawner()
		{
			PickupSpawner = IoC.GetInstance<PickupSpawner>();
			GibSpawner = IoC.GetInstance<GibSpawner>();
		}

		public void Setup(string gibSheetName)
		{
			GibSpawner.Setup(gibSheetName);
		}

		public void Spawn(Vector2f at)
		{
			GibSpawner.SpawnAll(at);
			PickupSpawner.SpawnAmount(at, Rng.RangeInt(MinAmountCoins, MaxAmountCoins), ChanceOfHeartDrop > Rng.Float() ? 1 : 0);
		}
	}
}
