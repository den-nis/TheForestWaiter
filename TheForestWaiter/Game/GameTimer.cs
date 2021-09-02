using System;
using System.Collections.Generic;
using System.Text;

namespace TheForestWaiter.Game
{
    class GameTimer
    {
        public GameTimer(TimeSpan interval) : this(interval, true)
        {
              
        }

        public GameTimer(TimeSpan interval, bool startEnabled)
        {
            Interval = interval;
            Enabled = startEnabled;
        }
       
        public bool Enabled { get; set; }

        private float Time { get; set; }

        public TimeSpan Interval { get; set; }

        public event Action<GameTimer> OnTick = delegate { };

        public void Update(float time)
        {
            if (Enabled)
            {
                Time += time;
                if (Time > (float)Interval.TotalSeconds)
                {
                    OnTick(this);
                    Time %= (float)Interval.TotalSeconds;
                }
            }
            else
            {
                Time = 0;
            }
        }
    }
}
