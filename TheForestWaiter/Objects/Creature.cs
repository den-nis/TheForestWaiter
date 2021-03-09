﻿using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entities;

namespace TheForestWaiter.Objects
{
    abstract class Creature : DynamicObject
    {
        public bool Dead { get; private set; }
        public float Health { get; protected set; } = 100;
        public bool IsStunned => _stunTimer > 0;

        protected float StunTime { get; set; } = (float)TimeSpan.FromSeconds(0.5f).TotalSeconds;
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

            base.Update(time);
        }

        public void Damage(DynamicObject by, float amount) => Damage(by, amount, StunTime);

        public void Damage(DynamicObject by, float amount, float stunTime)
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

        protected void ApplyKnockback(DynamicObject by)
        {
            var delta = (Center - by.Center).Norm() * Knockback;
            velocity = delta;
        }

        protected abstract void OnDeath();

        protected abstract void OnDamage(DynamicObject by);
    }
}
