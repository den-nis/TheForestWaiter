using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Environment;

namespace TheForestWaiter.Game.Objects.Static
{
    class Spawner : StaticObject
    {
        private readonly ObjectCreator _creator;
        private bool _initialUpdate = false;
        private int _amount;
        private int _interval;
        private Type _spawnType;

        public Spawner(GameData game, ObjectCreator creator) : base(game)
        {
            _creator = creator;
        }

        public override void Draw(RenderWindow window)
        {
        }

        public override void Update(float time)
        {
            if (!_initialUpdate)
            {
                OnFirstUpdate();
            }

            _initialUpdate = true;
        }

        private void OnFirstUpdate()
        {
            Spawn();
        }

        private void Spawn()
        {
            for (int i = 0; i < _amount; i++)
            {
                var obj = _creator.CreateType(_spawnType);
                obj.Position = Position;
                Game.Objects.AddAuto(obj);
            }
        }

        public override void MapSetup(MapObject mapObject)
        {
            var lookup = mapObject.Properties.ToDictionary(k => k.Name, v => v.Value);
            _amount = int.Parse(lookup["Amount"]);
            _interval = int.Parse(lookup["Interval"]);
            var name = lookup["Object"];

            Types.GameObjects.TryGetValue(name, out _spawnType);
        }
    }
}
