using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class ShopHud : HudSection
	{
		private const int ROWS = 3;
		private const int COLUMNS = 2;

		private readonly Dictionary<string, Sprite> _icons = new();
		private readonly Sprite _slot;
		private readonly ItemShop _shop;
		private readonly ContentSource _content;
		private readonly SpriteFont _priceText;

		public ShopHud(ContentSource content, ItemShop shop)
		{
			_content = content;
			_shop = shop;
			_slot = content.Textures.CreateSprite("Textures/Hud/shop_slot.png");
			_priceText = new SpriteFont(content.Textures.CreateSpriteSheet("Textures/Hud/shop_numbers.png"));

			Size = new Vector2f(_slot.Texture.Size.X* COLUMNS, _slot.Texture.Size.Y * ROWS);
		}

		public override bool IsMouseCaptured() => false;

		public override void Draw(RenderWindow window)
		{
			_slot.Scale = ScaleVector;

			var position = GetPosition(window);
			float offsetX = position.X;
			float offsetY = position.Y;

			int column = 0;
			foreach (var product in _shop.Products)
			{
				if (column >= COLUMNS)
				{
					column = 0;
					offsetX = position.X;
					offsetY += _slot.Texture.Size.Y * Scale;
				}

				_slot.Position = new Vector2f(offsetX, offsetY);
				window.Draw(_slot);

				var icon = GetIconSprite(product.IconSource);
				icon.Scale = ScaleVector;
				icon.Origin = (icon.Texture.Size / 2).ToVector2f();
				icon.Position = _slot.Position + (_slot.Texture.Size / 2).ToVector2f() * Scale;

				if (product.IsAvailable())
				{
					icon.Color = new Color(255, 255, 255, 255);
				}
				else
				{
					icon.Color = new Color(255, 255, 255, 100);
				}

				window.Draw(icon);
				offsetX += (int)(_slot.Texture.Size.X * Scale);
				column++;
			}
		}

		private Sprite GetIconSprite(string name)
		{
			if (_icons.TryGetValue(name, out var sprite))
			{
				return sprite;
			}
			else
			{
				var icon = _content.Textures.CreateSprite(name);
				_icons.Add(name, icon);
				return icon;
			}
		}

		public override void OnPrimaryReleased()
		{
			throw new NotImplementedException();
		}

		public override void Hover(Vector2f mouse)
		{
			throw new NotImplementedException();
		}
	}
}
