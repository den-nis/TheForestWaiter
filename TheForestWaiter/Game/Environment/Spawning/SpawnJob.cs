using SFML.System;
using System;
using System.Diagnostics;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Environment.Spawning
{
	internal class SpawnJob
	{
		public bool IsActive => _spawned < _description.Amount;

		private readonly SpawnJobDescription _description;
		private readonly Vector2f _leftSpawn;
		private readonly Vector2f _rightSpawn;

		private readonly GameData _gameData;
		private readonly ObjectCreator _creator;

		private int _spawned = 0;
		private float _delay = 0;

		public SpawnJob(SpawnJobDescription description, Vector2f leftSpawn, Vector2f rightSpawn)
		{
			_gameData = IoC.GetInstance<GameData>();
			_creator = IoC.GetInstance<ObjectCreator>();

			_description = description;
			_leftSpawn = leftSpawn;
			_rightSpawn = rightSpawn;
		}

		public void Update(float time)
		{
			if (!IsActive) return;

			_delay -= time;
			while (_delay <= 0)
			{
				_delay += _description.Delay;
				_spawned++;

				if (Types.GameObjects.TryGetValue(_description.Creature, out Type cType))
				{
					var creature = _creator.CreateType(cType);
					Debug.Assert(creature is Creature, "Type is not a creature");
					Spawn(creature);
				}
				else
				{
					Debug.Fail($"Spawner cannot find type \"{_description.Creature}\"");
				}

				if (_spawned >= _description.Amount) break;
			}
		}

		private void Spawn(GameObject creature)
		{
			creature.Position = PickSide();
			_gameData.Objects.AddGameObject(creature);
		}

		private Vector2f PickSide()
		{
			return _description.Side switch
			{
				SpawnSide.Both => Rng.Bool() ? _leftSpawn : _rightSpawn,
				SpawnSide.Right => _rightSpawn,
				SpawnSide.Left => _leftSpawn,
				_ => throw new NotImplementedException($"Not supported {_description.Side}"),
			};
		}
	}
}
