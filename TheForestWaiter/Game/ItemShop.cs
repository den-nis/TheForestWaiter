using LightInject;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects.Weapons.Abstract;

namespace TheForestWaiter.Game
{
	internal interface IDisplayProduct
	{
		int Id { get; }
		int Price { get; }
		string Name { get; }
		string IconSource { get; }

		bool IsAvailable();
	}

	internal class Product : IDisplayProduct
	{
		public int Id { get; set; }
		public string Name { get; set; } = "No name";
		public string Type { get; set; } = "None";
		public int Price { get; set; } = 1000;
		public bool AllowMultiplePurchases { get; set; } = false;
		public bool CanBeEquiped { get; set; } = true;
		public string IconSource { get; set; }

		private int _timesBought = 0;

		public bool IsAvailable()
		{
			if (AllowMultiplePurchases)
			{
				return true;
			}
			else
			{
				return _timesBought == 0;
			}
		}

		public bool CanAfford(int balance)
		{
			return Price <= balance;
		}

		public void Buy()
		{
			_timesBought++;
		}
	}

	internal class ItemShop
	{
		public IReadOnlyList<IDisplayProduct> Products => _products;
		private List<Product> _products = new();
		private readonly GameData _game;
		private readonly ServiceContainer _container;

		public ItemShop(GameData game, ContentSource content, ServiceContainer container)
		{
			_game = game;
			_container = container;
			Load(content.Source.GetString("shop.json"));
		}

		private void Load(string json)
		{
			_products = JsonConvert.DeserializeObject<List<Product>>(json);
			Debug.Assert(_products.ToLookup(k => k.Name).Any(l => l.Count() > 1), "Detected duplicate names");
		}
		
		/// <summary>
		/// Returns a message about the purchase
		/// </summary>
		public string Purchase(int id)
		{
			var product = _products.First(p => p.Id == id);

			if (!product.IsAvailable())
				return "Item is not available";

			if (!product.CanAfford(_game.Session.Coins))
				return "Insufficient funds";

			product.Buy();
			ApplyProduct(product);
			_game.Session.Coins -= product.Price;

			return $"Bought 1 x {product.Name}";
		}

		private void ApplyProduct(Product product)
		{
			switch (product.Type)
			{
				case "Weapon":
					var weapon = _container.GetInstance(Types.Weapons[product.Name]) as Weapon;
					_game.Objects.Player.Weapons.Add(weapon);
					break;

				default:
					throw new KeyNotFoundException($"Cannot handle product type \"{product.Type}\""); 
			}
		}
	}
}
