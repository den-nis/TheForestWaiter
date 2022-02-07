using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Core;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Weapons.Bullets;

namespace TheForestWaiter.Game.Objects.Enemies
{
    abstract class Enemy : Creature
    {
        public float Speed { get; set; } = 150;

        /// <summary>
        /// Time it takes for the enemy to become calm
        /// </summary>
        public float CalmTime { get; set; } = (float)TimeSpan.FromSeconds(5).TotalSeconds;

        /// <summary>
        /// Radius where the enemy gets angry
        /// </summary>
        public float AngerRadius { get; set; } = 200;
		public bool IsAngry { get; private set; } = false;

		private float _angerTimer = 0;

		protected Enemy(GameData game) : base(game)
        {
            Gravity = 2000;
        }

        public override void Update(float time)
        {
            _angerTimer -= time;
            if (_angerTimer <= 0)
            {
                IsAngry = false;
                var delta = Position - Game.Objects.Player.Position;
                var distance = delta.Len();

                if (distance < AngerRadius)
                {
                    Trigger();
                }
            }

            if (IsAngry)
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
            IsAngry = true;
            _angerTimer = CalmTime;
        }

        protected override void OnDamage(PhysicsObject by)
        {
            Trigger();
        }

		protected override void OnTouch(PhysicsObject obj)
		{
			if (obj is Player)
            {
                Trigger();
			}
		}

		public abstract void OnIdle(float time);
        public abstract void OnAngry(float time);

        /// <summary>
        /// Returns the signed value (direction) which it has advanced to 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="jump"></param>
        /// <returns></returns>
        protected int AdvanceToPlayerOnGround(float time, Action jump)
        {
            if (Math.Abs(RealSpeed.X) < (Speed / 10) * time)
                jump();

            var direction = (Game.Objects.Player.Center - Center).X;
            direction = Math.Max(-1, Math.Min(1, direction));
            LimitPush(new Vector2f(direction, 0) * Speed, time);
            return Math.Sign(direction);
        }
    }
}
