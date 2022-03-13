using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;

namespace TheForestWaiter.Game.Hud.Sections
{
	internal class ShopHud : HudSection
	{
		private class ShopSlot
        {
            public IDisplayProduct Product { get; set; }
			public Vector2f RelativePosition { get; set; }
        }

        private const int ROWS = 3;
		private const int COLUMNS = 2;

		private readonly Dictionary<string, Sprite> _iconCache = new();
		private readonly List<ShopSlot> _slots = new();	
        private readonly ContentSource _content;
		private readonly ItemShop _shop;
        private readonly Camera _camera;

		private readonly Sprite _slot;
		private readonly SpriteFont _priceText;

		private readonly Vector2f _priceOffset = new(-11, 4);

		private int _clickedOnIndex = -1;
		private int _mouseOnIndex = -1;

		public ShopHud(float scale, ContentSource content, ItemShop shop, Camera camera) : base(scale)
		{
			_content = content;
			_shop = shop;
            _camera = camera;

			_slot = content.Textures.CreateSprite("Textures/Hud/shop_slot.png");
			_priceText = new SpriteFont(content.Textures.CreateSpriteSheet("Textures/Hud/shop_numbers.png"));

			Size = new Vector2f(_slot.Texture.Size.X * COLUMNS, _slot.Texture.Size.Y * ROWS);

			SetupSlots();
		}

		public override void Draw(RenderWindow window)
		{
			for (int i = 0; i < _slots.Count; i++)
            {
				var slot = _slots[i];

				var position = GetPosition(window) + slot.RelativePosition * Scale;
				var icon = GetIconSprite(slot.Product.IconSource);
				var slotCenter = position + _slot.Texture.Size.ToVector2f() * Scale / 2;
				var color = GetSlotColor(slot, i);

				_priceText.Scale = Scale;
				_priceText.Position = slotCenter + _priceOffset * Scale;
				_priceText.SetText(slot.Product.Price.ToString());
				_priceText.Color = color;

				icon.Scale = ScaleVector;
				icon.Position = slotCenter;
				icon.Origin = icon.Texture.Size.ToVector2f() / 2;
				icon.Color = color;
				
				_slot.Scale = ScaleVector;
				_slot.Position = position;
				_slot.Color = color;

				window.Draw(_slot);
				window.Draw(icon);
				_priceText.Draw(window);
            }
		}

		private Color GetSlotColor(ShopSlot slot, int index)
        {
			if (!slot.Product.IsAvailable())
			{
				return new Color(255, 255, 255, 150);
			}

			if (_mouseOnIndex == index)
            {
				return new Color(200, 200, 255, 255);
            }

			return Color.White;
        }

		private void SetupSlots()
		{
			for (int y = 0; y < ROWS; y++)
			{
				for (int x = 0; x < COLUMNS; x++)
				{
					int i = y * COLUMNS + x;
					var item = _shop.Products[i];

					_slots.Add(new ShopSlot()
					{
						Product = item,
						RelativePosition = _slot.Texture.Size.ToVector2f().Multiply(new Vector2f(x, y))
					});
				}
			}
		}

		private Sprite GetIconSprite(string name)
		{
			if (_iconCache.TryGetValue(name, out var sprite))
			{
				return sprite;
			}
			else
			{
				var icon = _content.Textures.CreateSprite(name);
				_iconCache.Add(name, icon);
				return icon;
			}
		}

		public override void OnMouseMove(Vector2i mouse)
        {
			for (int i = 0; i < _slots.Count; i++)
            {
				var slot = _slots[i];
				var rect = new FloatRect(
					GetPosition(_camera) + slot.RelativePosition * Scale,
					_slot.Texture.Size.ToVector2f() * Scale);

				if (rect.Intersects(new FloatRect(mouse.ToVector2f(), new Vector2f(1, 1))))
                {
					_mouseOnIndex = i;
					return;
				}
            }

			_mouseOnIndex = -1;
        }

		public override bool IsMouseOnAnyButton()
		{
			if (_mouseOnIndex != -1)
            {
				return true;
            }

			return false;
		}

		public override void OnPrimaryReleased() 
		{
			if (_clickedOnIndex != -1 && _clickedOnIndex == _mouseOnIndex)
			{
				//TODO: do something with the responses?
				_shop.Purchase(_slots[_clickedOnIndex].Product.Id);
			}
        }

        public override void OnPrimaryPressed() 
		{
			_clickedOnIndex = _mouseOnIndex;
		}
    }
}
