using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Environment.Spawning;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Static
{
	internal class Spawner : Immovable
	{
        private class InnerSpawner
        {
            public bool Enabled { get; set; } = true;

            private Vector2f _left;
            private Vector2f _right;

            private readonly Timer _timer;
			private readonly GameData _game;
			private readonly SpawnJob _job;
			private readonly ObjectCreator _creator;

			public InnerSpawner(GameData gameData, SpawnJob job, ObjectCreator creator, Vector2f left, Vector2f right)
			{
                _left = left;
                _right = right;
                
				_creator = creator;
				_game = gameData;
				_job = job;

                _timer = new Timer(1 / job.Rate);
				_timer.OnTick += OnSpawn;
                _timer.Start();
			}

            public void Update(float time)
            {
                _timer.Update(time);
			}

			private void OnSpawn()
			{
                if (!Enabled)
                    return;

				var location = Rng.Bool() ? _left : _right;
				var enemy = _creator.CreateType(Types.GameObjects[_job.Name]);
                enemy.Center = location;

				_game.Objects.AddGameObject(enemy);
            }
		}

		private const int WAVE_LENGTH = 50;

		private readonly WaveSettings _settings;
        private int _currentWave = 1;
        private float _waveTimer = 0; 
        private float _checkTimer = 0;

        private readonly List<InnerSpawner> _spawners = new();
		private readonly ObjectCreator _creator;

		public Spawner()
		{
            var content = IoC.GetInstance<ContentSource>();
            _creator = IoC.GetInstance<ObjectCreator>();
            
			var json = content.Source.GetString("waveSettings.json");
			_settings = WaveSettings.FromJson(json);
		}

        public int GetCurrentWave() => _currentWave;

		public override void Draw(RenderWindow window)
        {
            
        }

        public override void Update(float time)
        {
            //Setup the first wave
            if (_waveTimer == 0 && _currentWave == 1)
            {
                SetupWave();
			}

            _waveTimer += time;
            _checkTimer += time;
            
            if (_checkTimer > 1)
            {
                bool allEnemiesArekilled = !Game.Objects.Creatures.Any(x => !x.Friendly);
                bool timeIsUp = _waveTimer > WAVE_LENGTH;

                if (timeIsUp)
                {
                    if (_spawners[0].Enabled)
                        _spawners.ForEach(x => x.Enabled = false);

                    if (allEnemiesArekilled)
                    {
                        _currentWave++;
                        Console.WriteLine($"Wave finished. Starting wave {_currentWave}");
                        SetupWave();
                    }
                }

                _checkTimer = 0;
            }

            foreach(var spawner in _spawners)
            {
                spawner.Update(time);
			}
        }

        private void SetupWave()
        {
            _waveTimer = 0;
            var enemies = _settings.Waves[_currentWave - 1].Enemies;

            _spawners.Clear();
            foreach (var enemy in enemies)
            {
				Console.WriteLine($"Creating spawner for {enemy.Name}");
                var spawner = new InnerSpawner(Game, enemy, _creator, _settings.LeftSpawn, _settings.RightSpawn);
                _spawners.Add(spawner);
            }
        }
    }
}
