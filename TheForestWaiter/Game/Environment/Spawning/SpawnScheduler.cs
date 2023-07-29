using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Content;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal class SpawnScheduler : ISetup
	{
        public class Job
        {
            public string EnemyName { get; set; }
            public SpawnSide Side { get; set; }
        }

        private float waveTime = 0;
        private int budget = 0;

        private ContentSource content;
        private SpawnContext context;
        private GameData gameData;

        private float[] intervals = new float[0];
        private string[] enemyNames = new string[0]; 
        private int index = 0;
        private float intervalProcess = 0;

        public int WaveNumber { get; private set; } = 1;
        public Queue<Job> Jobs { get; } = new Queue<Job>();

        public SpawnScheduler()
        {
            content = IoC.GetInstance<ContentSource>();
            context = IoC.GetInstance<SpawnContext>();
            gameData = IoC.GetInstance<GameData>();
        }

		public void Setup()
		{
			budget = context.InitialBudget;
            StartNewWave();
		}

        public void FastForwardToWave(int wave)
        {
            FinishCurrentWave();
            WaveNumber = wave;
            budget = context.InitialBudget + wave * context.IncreaseBudget;
            StartNewWave();
        }

		public void Update(float time)
        {
            waveTime += time;
            intervalProcess += time;

            if (waveTime >= context.WaveDuration)
            {
                if (!HasEnemiesLeft())
                {
                    FinishCurrentWave();
                    StartNewWave();
                }
            }
            else
            {
                while (index < intervals.Length && intervalProcess >= intervals[index] / 2)
                {
                    Spawn(enemyNames[index]);
                    intervalProcess = intervalProcess - intervals[index];
                    index++;
                }
            }
        }

        private void Spawn(string name) 
        {
            SpawnSide at = Rng.Pick(new[] { SpawnSide.Left, SpawnSide.Right });
            
            Jobs.Enqueue(new Job
            {
                EnemyName = name,
                Side = at,
            });
        }

        private void StartNewWave()
        {
            ISpawnPicker spawnPicker = SpawnStrategy.GetRandomSpawnPicker();
            ISpawnIntervals spawnIntervals = SpawnStrategy.GetRandomSpawnIntervals();

            enemyNames = spawnPicker.Generate(budget, WaveNumber).ToArray();
            intervals = spawnIntervals.Generate(enemyNames.Length).Select(x => x * context.WaveDuration).ToArray();

            index = 0;
            intervalProcess = 0;
            waveTime = 0;
        }

        private void FinishCurrentWave() 
        {
            WaveNumber++;
            budget += context.IncreaseBudget;
        }

		private bool HasEnemiesLeft() => gameData.Objects.Creatures.Any(c => !c.Friendly);
	}
}