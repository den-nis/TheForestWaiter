using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Environment.Spawning;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Objects.Items;

namespace TheForestWaiter.Game.Objects.Static
{
	internal class Spawner : Immovable
	{
		private const float CHECK_INTERVAL = 1;

		public int CurrentWave { get; private set; } = 0;
		private float _waveTime = 0;
		private float _checkTimer = 0;

		private readonly WaveSettings _settings;
		private readonly List<SpawnJob> _activeJobs = new();
		private List<SpawnJobDescription> _spawnQueue = new();
		private bool _finished = false;

		public Spawner()
		{
			var content = IoC.GetInstance<ContentSource>();

			var json = content.Source.GetString("waveSettings.json");
			_settings = WaveSettings.FromJson(json);

			StartWave(1);
		}

		public void StartWave(int number)
		{
			CurrentWave = number;
			_waveTime = 0;
			_spawnQueue = new(GetJobsForWave(number).OrderBy(j => j.Time));

			if (_spawnQueue.Count == 0) _finished = true;

			PickupSpawner.EnableHearts = number < 10;
		}

		public override void Draw(RenderWindow window)
		{

		}

		public override void Update(float time)
		{
			_checkTimer += time;
			_waveTime += time;

			if (_finished) return;

			var shouldStart = _spawnQueue.Where(w => w.Time < _waveTime);
			foreach(var job in shouldStart)
			{
				StartJob(job);
			}

			_spawnQueue.RemoveAll(j => shouldStart.Contains(j));

			foreach (var job in _activeJobs)
			{
				job.Update(time);
			}

			if (_checkTimer > CHECK_INTERVAL)
			{
				_activeJobs.RemoveAll(j => !j.IsActive);
				_checkTimer = 0;

				if (_spawnQueue.Count == 0 && GetAmountOfEnemiesLeft() == 0)
				{
					StartWave(CurrentWave + 1);
				}
			}
		}

		private void StartJob(SpawnJobDescription job)
		{
			var spawnJob = new SpawnJob(job, _settings.LeftSpawn, _settings.RightSpawn);
			_activeJobs.Add(spawnJob);
		}

		private int GetAmountOfEnemiesLeft()
		{
			var enemies = Game.Objects.Creatures.Where(c => !c.Friendly);
			return enemies.Count();
		}

		/// <summary>
		/// Returns empty list if the wave isn't programmed
		/// </summary>
		private IEnumerable<SpawnJobDescription> GetJobsForWave(int waveNumber)
		{
			if (waveNumber > _settings.Waves.Length)
			{
				return Enumerable.Empty<SpawnJobDescription>();
			}
			else
			{
				return _settings.Waves[waveNumber - 1].Jobs;
			}
		}
	}
}
