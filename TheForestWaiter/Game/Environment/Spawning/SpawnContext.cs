using Newtonsoft.Json;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal partial class SpawnContext : ISetup
	{
        private class SpawnSettings 
        {
            [JsonConverter(typeof(Vector2fValueConverter))]
            public Vector2f LeftSpawn { get; set; }

            [JsonConverter(typeof(Vector2fValueConverter))]
            public Vector2f RightSpawn { get; set; } 

            public int WaveDuration { get; set; }
            public int InitialBudget { get; set; }
            public int BudgetIncrement { get; set; }

            public string[] Spawnable { get; set; }
            public Dictionary<string, int> EnemyBudget { get; set; }
            public Dictionary<string, int> EnemyIntroductionAtWave { get; set; }
            public Dictionary<string, float> PickerChances { get; set; }
            public Dictionary<string, float> IntervalChances { get; set; }
        }

        public int InitialBudget => settings.InitialBudget;
        public int IncreaseBudget => settings.BudgetIncrement;
        public int WaveDuration => settings.WaveDuration;
    
        private SpawnSettings settings;
        private ContentSource content;

        public SpawnContext()
        {
           content = IoC.GetInstance<ContentSource>(); 
        }

		public void Setup()
		{
			var json = content.Source.GetString("spawn_settings.json");
            settings = JsonConvert.DeserializeObject<SpawnSettings>(json);
		}

        public Vector2f GetPosition(SpawnSide side)
        {
            switch (side)
            {
                case SpawnSide.Left: return settings.LeftSpawn;
                case SpawnSide.Right: return settings.RightSpawn;
            }

            throw new ArgumentException($"Cannot find position for side \"{side}\"");
        }

        public int GetCost(string enemy) 
        {
            return settings.EnemyBudget[enemy];
        }
        
        public IEnumerable<string> GetSpawnableEnemies(int wave) 
        {
            return settings.Spawnable.Where(x => settings.EnemyIntroductionAtWave[x] <= wave);
        }
	}
}