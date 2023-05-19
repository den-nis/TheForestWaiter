using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class WeaponsHud : HudSection
	{
		private const float SCALE_ON_SELECT = 1.2f;

		private readonly Dictionary<string, Sprite> _weaponSprites = new();
		private readonly Sprite _slot;
		private readonly Sprite _select;
		private readonly Camera _camera;
		private readonly GameData _game;
		private readonly ContentSource _content;
		private readonly float radius;

		public WeaponsHud(float scale) : base(scale)
		{
			_camera = IoC.GetInstance<Camera>();
			_game = IoC.GetInstance<GameData>();
			_content = IoC.GetInstance<ContentSource>();
			_slot = _content.Textures.CreateSprite("Textures/Hud/slot.png");
			_select = _content.Textures.CreateSprite("Textures/Hud/select.png");
			radius = IoC.GetInstance<UserSettings>().GetFloat("Game", "WeaponWheelRadius");

			Size = new Vector2f(0, 0);
		}

		public override void Draw(RenderWindow window)
		{
			if (_game.Objects.Player.WeaponWheel.IsSelecting)
			{
				DrawWeaponWheel(window);
			}
		}

		private void DrawWeaponWheel(RenderWindow window)
		{	
			foreach (var item in _game.Objects.Player.WeaponWheel.Items) 
			{
				_select.Scale = item.IsInFocus ? ScaleVector * SCALE_ON_SELECT : ScaleVector;
				_slot.Scale = item.IsInFocus ? ScaleVector * SCALE_ON_SELECT : ScaleVector;

				var offset = TrigHelper.FromAngleRad(item.Angle, radius * Scale);
				var position = _game.Objects.Player.WeaponWheel.GetAnchor().ToVector2f() + offset;

				_slot.Origin = (_slot.Texture.Size / 2).ToVector2f();
				_slot.Position = position;
				window.Draw(_slot);

				if (item.IsInFocus)
				{
					_select.Origin = (_select.Texture.Size / 2).ToVector2f();
					_select.Position = position;
					window.Draw(_select);
				}
				
				if (!item.IsEmpty)
				{
					var icon = GetIconSprite(item.Weapon.IconTextureName);
					icon.Scale = item.IsInFocus ? ScaleVector * SCALE_ON_SELECT : ScaleVector;
					icon.Origin = (icon.Texture.Size / 2).ToVector2f();
					icon.Position = _slot.Position;
					window.Draw(icon);
				}
			}
		}

		public override bool IsMouseOnAnyButton() => false;

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

		public override void OnPrimaryReleased() { }
		public override void OnPrimaryPressed() { }
		public override void OnMouseMove(Vector2i mouse) { }
	}
}