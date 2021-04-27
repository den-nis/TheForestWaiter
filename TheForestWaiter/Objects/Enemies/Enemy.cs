using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entities;
using TheForestWaiter.Essentials;

namespace TheForestWaiter.Objects.Enemies
{
    abstract class Enemy : Creature
    {
        public Enemy(GameData game) : base(game)
        {
            Gravity = 2000;
        }

        public float TouchingDamage { get; set; } = 1;
        public float TouchingKnockback { get; set; } = 100;
        public float Speed { get; set; } = 150;
        public float JumpVelocity { get; set; } = 300;

        /// <summary>
        /// Time it takes for the enemy to become calm
        /// </summary>
        public float CalmTime { get; set; } = (float)TimeSpan.FromSeconds(5).TotalSeconds;

        /// <summary>
        /// Radius where the enemy gets angry
        /// </summary>
        public float TriggeredRadius { get; set; } = 200;
        public bool IsTriggered => _triggered;
        private float _triggeredTimer = 0;
        private bool _triggered = false;

        public override void Update(float time)
        {
            _triggeredTimer -= time;
            if (_triggeredTimer <= 0)
            {
                _triggered = false;
                var delta = Position - Game.Objects.Player.Position;
                var distance = delta.Len();

                if (distance < TriggeredRadius)
                {
                    Trigger();
                }
            }

            if (_triggered)
            {
                OnAngry(time);
            }
            else
            {
                OnIdle(time);
            }

            base.Update(time);
        }

        private void Trigger()
        {
            _triggered = true;
            _triggeredTimer = CalmTime;
        }

        protected override void OnDamage(DynamicObject by)
        {
            Trigger();
        }

        public abstract void OnIdle(float time);
        public abstract void OnAngry(float time);
    }
}
