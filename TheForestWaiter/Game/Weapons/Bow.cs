﻿using SFML.System;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Objects.Projectiles;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Weapons
{
	internal class Bow : Weapon
	{
		public override string IconTextureName => "Textures/Weapons/Icons/bow.png";

		protected override Vector2f AttachPoint => _gameData.Objects.Player.Center - new Vector2f(0, 1);
		protected override Vector2f Origin => new(2.5f, 8.5f);

		private readonly GameData _gameData;

		public Bow()
		{
			var content = IoC.GetInstance<ContentSource>();
			_gameData = IoC.GetInstance<GameData>();

			AutoFire = false;
			Cone = TrigHelper.ToRad(5);
			FireRatePerSecond = 2;
			FireSpeedVariation = 10;
			FireSpeed = 500;

			Weight = 0;

			FireSound = new("Sounds/Weapons/bow.wav");
			Sprite = content.Textures.CreateSprite("Textures/Weapons/bow.png");

			FireSound.PitchVariation = 0.05f;
		}

		public override void OnFire()
		{
			FireProjectile<Arrow>();
		}
	}
}
