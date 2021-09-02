using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Game.Essentials
{
    class RandomTrigger
    {
        private readonly Action _trigger;
        private readonly float _chance;
        private readonly float _interval;
        private float _timer;

        public RandomTrigger(Action trigger, float percentChance, float interval)
        {
            _trigger = trigger;
            _chance = percentChance;
            _interval = interval;
        }

        public void TryTrigger(float time)
        {
            _timer += time;

            while (_timer > _interval)
            {
                if (Rng.Range(0, 100) <= _chance)
                    _trigger();

                _timer -= _interval;
            }
        }
    }
}
