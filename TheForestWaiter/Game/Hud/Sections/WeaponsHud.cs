using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class WeaponsHud : HudSection
	{
		private const int MARGIN_WEAPON_SLOTS = 10;

		private readonly Dictionary<string, Sprite> _weaponSprites = new();
		private readonly Sprite _slot;
		private readonly Sprite _select;
		private readonly GameData _game;
		private readonly ContentSource _content;

		public WeaponsHud(GameData game, ContentSource content)
		{
			_game = game;
			_content = content;
			_slot = content.Textures.CreateSprite("Textures/Hud/slot.png");
			_select = content.Textures.CreateSprite("Textures/Hud/select.png");

			Size = new Vector2f(_slot.Texture.Size.X * 9, _slot.Texture.Size.Y);
		}

		public override void Draw(RenderWindow window)
		{
			_select.Scale = ScaleVector;
			_slot.Scale = ScaleVector;

			var position = GetPosition(window);
			float offset = position.X;

			foreach (var weapon in _game.Objects.Player.Weapons.OwnedWeapons)
			{
				_slot.Position = new Vector2f(offset, position.Y);
				_select.Position = _slot.Position;
				window.Draw(_slot);

				var icon = GetIconSprite(weapon.IconTextureName);
				icon.Scale = ScaleVector;
				icon.Origin = (icon.Texture.Size / 2).ToVector2f();
				icon.Position = _slot.Position + (_slot.Texture.Size / 2).ToVector2f() * Scale;
				window.Draw(icon);

				if (weapon == _game.Objects.Player.Weapons.GetEquiped())
				{
					window.Draw(_select);
				}

				offset += (int)(_slot.Texture.Size.X * Scale) + MARGIN_WEAPON_SLOTS;
			}
		}

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
	}
}
