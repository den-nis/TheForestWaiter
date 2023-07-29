using SFML.Graphics;
using System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Logic;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Graphics;

namespace TheForestWaiter.Game.Objects.Enemies
{
	//TODO: Implement attack animation
	internal class Vortex : FlyingCreature
	{
		private const float ATTACK_X_DISTANCE = 400;
		private const float ATTACK_DAMAGE = 15;
		private const float KNOCKBACK = 100;

		private readonly AnimatedSprite _animation;
		private readonly ContentSource _content;
		private readonly DropSpawner _dropSpawner;

		private readonly float _maxFlyingHeight;
		private readonly float _minFlyingHeight;

		public Vortex()
		{
			_content = IoC.GetInstance<ContentSource>();
			_dropSpawner = IoC.GetInstance<DropSpawner>();

			_dropSpawner.Setup("Textures/Enemies/vortex_gibs.png");

			_animation = _content.Textures.CreateAnimatedSprite("Textures/Enemies/vortex.png");
			_animation.SetSection("flying");
			_animation.Framerate += Rng.Float();

			Size = _animation.Sheet.Rect.CellSize.ToVector2f();

			_maxFlyingHeight = 180 + Rng.Var(80);
			_minFlyingHeight = 50;
			FlyingHeight = _maxFlyingHeight;

			Speed = 220 + Rng.Var(40);
			SetMaxHealth(300, true);
		}

		public override void Update(float time)
		{
			base.Update(time);

			var distance = Math.Abs(Game.Objects.Player.Center.X - Center.X);
			if (distance < ATTACK_X_DISTANCE)
			{
				FlyingHeight = Math.Max(_minFlyingHeight, distance);
			}
			else
			{
				FlyingHeight = _maxFlyingHeight;
			}

			Chase(Game.Objects.Player);

			if (Intersects(Game.Objects.Player))
			{
				Game.Objects.Player.Damage(this, ATTACK_DAMAGE, KNOCKBACK);
			}

			HandleAnimations(time);
		}

		public void HandleAnimations(float time)
		{
			_animation.Sprite.Color = IsStunned ? new Color(255, 0, 0) : Color.White;
			_animation.Sprite.Position = Position;
			_animation.Sheet.Rect.MirrorX = FacingDirection > 0;

			_animation.Update(time);
		}

		public override void Draw(RenderWindow window)
		{
			window.Draw(_animation);
		}

		protected override void OnDeath()
		{
			var prop = _content.Particles.Get("Particles/blood.particle", Center);
			Game.Objects.WorldParticles.Emit(prop, 5);

			_dropSpawner.Spawn(Center);
			Delete();
		}

		protected override void OnDamage(GameObject by, float amount)
		{
			var prop = _content.Particles.Get("Particles/blood.particle", Center);
			Game.Objects.WorldParticles.Emit(prop, 5);
		}
	}
}