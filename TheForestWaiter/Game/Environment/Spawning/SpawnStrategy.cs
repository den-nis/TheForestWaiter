using TheForestWaiter.Game.Environment.Spawning.Intervals;
using TheForestWaiter.Game.Environment.Spawning.Pickers;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal static class SpawnStrategy
	{
        private static readonly ISpawnPicker[] picker = new ISpawnPicker[] {
            new RandomSpawnPicker(),
            new SpamSpawnPicker()
        };

        private static readonly ISpawnIntervals[] intervals = new ISpawnIntervals[] {
            new LinearSpawnIntervals(),
            new RandomSpawnIntervals(),
            new RushSpawnIntervals()
        };

        public static ISpawnPicker GetRandomSpawnPicker() => Rng.Pick(picker);

        public static ISpawnIntervals GetRandomSpawnIntervals() => Rng.Pick(intervals);
    }
}