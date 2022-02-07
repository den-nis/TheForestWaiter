using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Environment;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects.Static
{
    class Spawner : StaticObject
    {
		private readonly GameData _game;
		private readonly ObjectCreator _creator;
        private bool _initialUpdate = false;
        private int _amount;

        /// <summary>
        /// 0 = Anywhere 
        /// </summary>
        private Vector2i _triggerDistance = new(0,0);

        private readonly Timer _timer = new();
        private Type _spawnType;

        public Spawner(GameData game, ObjectCreator creator) : base(game)
        {
			_game = game;
			_creator = creator;
            _timer.OnTick += Spawn;
        }

        public override void Draw(RenderWindow window)
        {
        }

        public override void Update(float time)
        {
            _timer.Update(time);

            if (!_initialUpdate)
            {
                Spawn();
			}

			_initialUpdate = true;
        }

        private void Spawn()
        {
            var distance = (_game.Objects.Player.Center - Center).Abs();

            if ((distance.X < _triggerDistance.X || _triggerDistance.X == 0f) && (distance.Y < _triggerDistance.Y || _triggerDistance.Y == 0))
            {
                for (int i = 0; i < _amount; i++)
                {
                    var obj = _creator.CreateType(_spawnType);
                    obj.Position = Position;
                    Game.Objects.AddAuto(obj);
                }
            }
        }

        public override void MapSetup(MapObject mapObject)
        {
            var lookup = mapObject.Properties.ToDictionary(k => k.Name, v => v.Value);
            _amount = int.Parse(lookup["Amount"]);
            
            var triggerDistanceParts = lookup["TriggerDistance"].Split(',');
            _triggerDistance = new(
                int.Parse(triggerDistanceParts[0]) * World.TILE_SIZE, 
                int.Parse(triggerDistanceParts[1]) * World.TILE_SIZE
                );

            float interval = float.Parse(lookup["Interval"]);

            if (interval != 0.0f)
            {
                _timer.SetInterval(interval);
                _timer.OnTick += Spawn;
            }

            Types.GameObjects.TryGetValue(lookup["Object"], out _spawnType);
        }
    }
}
