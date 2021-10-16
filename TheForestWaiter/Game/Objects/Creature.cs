﻿using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Debugging;
using TheForestWaiter.Game.Debugging;
using TheForestWaiter.Game.Entities;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Objects
{
    abstract class Creature : PhysicsObject
    {
        public bool Dead { get; private set; }
        public float Health { get; protected set; } = 100;
        public bool IsStunned => _stunTimer > 0;

        protected float StunTime { get; set; } = 0.1f;
        protected float MaxHealth { get; set; } = 100;
        protected float Knockback { get; set; } = 400;
        protected float HealthPercentage => MaxHealth / Health;

        private float _stunTimer = 0;

        public Creature(GameData game) : base(game)
        {

        }

        public override void Update(float time)
        {
            if (_stunTimer > 0)
                _stunTimer -= time;
        }

        public void Damage(PhysicsObject by, float amount) => Damage(by, amount, StunTime);

        public void Damage(PhysicsObject by, float amount, float stunTime)
        {
            if (IsStunned)
                return;

            _stunTimer = stunTime;

            Health -= amount;
            Health = Math.Max(0, Health);

            if (Health <= 0 && !Dead)
            {
                Dead = true;
                OnDeath();
            }

            OnDamage(by);
        }

        protected void ApplyStunMovement(PhysicsObject by, float cone = (float)Math.PI/2)
        {
            var angle = (Center - by.Center).Angle();
            angle += Rng.Range(-cone, cone);
            Velocity = TrigHelper.FromAngleRad(angle, Knockback);
        }

        protected abstract void OnDeath();

        protected abstract void OnDamage(PhysicsObject by);
    }
}
