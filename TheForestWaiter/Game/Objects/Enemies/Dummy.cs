using SFML.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Enemies
{
	internal class Dummy : Creature
	{
		private const float SAMPLE_LENGTH = 1f;
		private readonly List<float> _history = new();
		private readonly Sprite _sprite;

		private float _totalDamage = 0;
		private bool _timerEnabled = false;
		private float _timer;

		public Dummy()
		{
			var content = IoC.GetInstance<ContentSource>();

			_sprite = content.Textures.CreateSprite("Textures/Enemies/dummy.png");
			Size = _sprite.Texture.Size.ToVector2f();

			KnockbackResistance = float.PositiveInfinity;

			SetMaxHealth(int.MaxValue, true);
		}

		public override void Draw(RenderWindow window)
		{
			_sprite.Position = Position;
			window.Draw(_sprite);
		}

		public override void Update(float time)
		{
			base.Update(time);

			if (_timerEnabled)
				_timer += time;

			if (_timer > SAMPLE_LENGTH)
			{
				var total = _history.Sum();
				var dps = total / SAMPLE_LENGTH;
				_totalDamage += total;

				Debug.WriteLine($"Dummy ({GameObjectId}) d/hit : {_history.Average(),-5} dps : {dps,-5} hits : {_history.Count,-5} total : {_totalDamage}");

				_history.Clear();
				_timerEnabled = false;
				_timer = 0;
			}
		}

		protected override void OnDamage(GameObject by, float damage)
		{
			if (!_timerEnabled)
				_timerEnabled = true;

			_history.Add(damage);
		}

		protected override void OnDeath()
		{
		}
	}
}
