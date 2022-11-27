﻿using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class WeaponsHud : HudSection
	{
		private const int MAX_WEAPONS = 9; //TODO: move this to Inventory (prevent buying more?)
		private const int MARGIN_WEAPON_SLOTS = 10;

		private readonly Dictionary<string, Sprite> _weaponSprites = new();
		private readonly Sprite _slot;
		private readonly Sprite _select;
		private readonly Camera _camera;
		private readonly GameData _game;
		private readonly ContentSource _content;
		private readonly ItemRepository _repo;
		private int _mouseOnIndex = 0;

		public WeaponsHud(float scale) : base(scale)
		{
			_camera = IoC.GetInstance<Camera>();
			_game = IoC.GetInstance<GameData>();
			_content = IoC.GetInstance<ContentSource>();
			_repo = IoC.GetInstance<ItemRepository>();
			_slot = _content.Textures.CreateSprite("Textures/Hud/slot.png");
			_select = _content.Textures.CreateSprite("Textures/Hud/select.png");

			Size = new Vector2f(_slot.Texture.Size.X * MAX_WEAPONS, _slot.Texture.Size.Y);
		}

		public override void Draw(RenderWindow window)
		{
			_select.Scale = ScaleVector;
			_slot.Scale = ScaleVector;

			LoopOverSlotPositions((position, info) =>
			{
				_slot.Position = position;
				_select.Position = _slot.Position;
				window.Draw(_slot);

				var icon = GetIconSprite(info.IconSource);
				icon.Scale = ScaleVector;
				icon.Origin = (icon.Texture.Size / 2).ToVector2f();
				icon.Position = _slot.Position + (_slot.Texture.Size / 2).ToVector2f() * Scale;
				window.Draw(icon);

				if (info.ItemId == _game.Objects.Player.Inventory.GetCurrentItem())
				{
					window.Draw(_select);
				}
			});
		}

		public override bool IsMouseOnAnyButton() => _mouseOnIndex != -1;

		private Sprite GetIconSprite(string name)
		{
			if (_weaponSprites.TryGetValue(name, out var sprite))
			{
				return sprite;
			}
			else
			{
				var icon = _content.Textures.CreateSprite(name);
				_weaponSprites.Add(name, icon);
				return icon;
			}
		}

		public override void OnPrimaryReleased()
		{
			if (_mouseOnIndex != -1)
			{
				_game.Objects.Player.Inventory.Select(_mouseOnIndex);
			}
		}

		public override void OnMouseMove(Vector2i mouse)
		{
			_mouseOnIndex = -1;
			LoopOverSlotPositions((position, info) =>
			{
				var rect = new FloatRect(position, _slot.Texture.Size.ToVector2f() * Scale);
				if (rect.Intersects(new FloatRect(mouse.ToVector2f(), new Vector2f(1, 1))))
				{
					//Casting because for some reason IReadonlyList does not have .IndexOf
					_mouseOnIndex = ((List<int>)_game.Objects.Player.Inventory.Items).IndexOf(info.ItemId);
					return;
				}
			});
		}

		public void LoopOverSlotPositions(Action<Vector2f, ItemInfo> func)
		{
			var section = GetPosition(_camera);
			float offset = section.X;

			foreach (var itemId in _game.Objects.Player.Inventory.Items)
			{
				var position = new Vector2f(offset, section.Y);

				func(position, _repo.GetItemInfo(itemId));

				offset += (int)(_slot.Texture.Size.X * Scale) + MARGIN_WEAPON_SLOTS;
			}
		}

		public override void OnPrimaryPressed() { }
	}
}
