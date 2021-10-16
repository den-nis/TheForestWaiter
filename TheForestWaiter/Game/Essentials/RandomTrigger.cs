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
        private readonly float _intervalMin;
        private readonly float _intervalMax;
        private float _interval;
        private float _timer;

        public RandomTrigger(Action trigger, float percentChance, float interval)
        {
            _trigger = trigger;
            _chance = percentChance;
            _interval = interval;
            _intervalMax = interval;
            _intervalMin = interval;
        }

        public RandomTrigger(Action trigger, float percentChance, float minInterval, float maxInterval)
        {
            _trigger = trigger;
            _chance = percentChance;
            _intervalMax = maxInterval;
            _intervalMin = minInterval;
            SetInterval();
        }

        public void TryTrigger(float time)
        {
            _timer += time;

            while (_timer > _interval)
            {
                if (Rng.Range(0, 100) <= _chance)
                    _trigger();

                SetInterval();
                _timer -= _interval;
            }
        }

        private void SetInterval()
        {
            _interval = Rng.Range(_intervalMin, _intervalMax);
        }
    }
}
