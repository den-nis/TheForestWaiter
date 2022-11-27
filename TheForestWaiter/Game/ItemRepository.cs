using LightInject;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Objects;
using TheForestWaiter.Multiplayer.Messages;

namespace TheForestWaiter.Game;

internal interface IDisplayProduct
{
	int ItemId { get; }
	int Price { get; }
	string Name { get; }
	string IconSource { get; }

	bool IsAvailable();
}

internal class ItemInfo : IDisplayProduct
{
	public int ItemId { get; set; }
	public string Name { get; set; } = "No name";
	public string Type { get; set; } = "None";
	public int Price { get; set; } = 1000;
	public bool IsInStore { get; set; }
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

	public void MarkBought()
	{
		_timesBought++;
	}
}

/// <summary>
/// Stores data about items
/// </summary>
internal class ItemRepository
{
	public IEnumerable<ItemInfo> All => _allItems;

	private List<ItemInfo> _allItems = new();
	private readonly GameData _game;
	private readonly ServiceContainer _container;
	private readonly NetworkTraffic _traffic;

	public ItemRepository(GameData game, ContentSource content, ServiceContainer container, NetworkTraffic traffic)
	{
		_game = game;
		_container = container;
		_traffic = traffic;
		Load(content.Source.GetString("items.json"));
	}

	/// <summary>
	/// List of items that can be bought
	/// </summary>
	public IReadOnlyList<IDisplayProduct> GetProducts()
	{
		return _allItems
			.Where(x => x.IsInStore)
			.ToList();
	}

	private void Load(string json)
	{
		_allItems = JsonConvert.DeserializeObject<List<ItemInfo>>(json);
		Debug.Assert(_allItems.ToLookup(k => k.ItemId).All(l => l.Count() == 1), "Duplicate ids");
	}

	public ItemInfo GetItemInfo(int itemId)
	{
		return _allItems.First(x => x.ItemId == itemId);
	}

	/// <summary>
	/// Returns a message about the purchase
	/// </summary>
	public string Purchase(int id)
	{
		var product = _allItems.First(p => p.ItemId == id);

		if (!product.IsAvailable())
			return "Item is not available";

		if (!product.CanAfford(_game.Session.Coins))
			return "Insufficient funds";

		product.MarkBought();
		Give(id);
		_game.Session.Coins -= product.Price;

		return $"Bought 1 x {product.Name}";
	}

	/// <summary>
	/// Get the product directly. No money needed
	/// </summary>
	public void Give(int id, Player player = null)
	{
		var p = (player ?? _game.Objects.Player);
		if (player == null || !player.IsGhost)
		{
			_traffic.SendIfMultiplayer(new PlayerItems
			{
				PlayerId = _traffic.MyId,
				Items = p.Inventory.Items.ToArray(),
				EquipedIndex = p.Inventory.EquipedIndex,
			});
		}

		var product = _allItems.First(p => p.ItemId == id);
		product.MarkBought();

		p.Inventory.Add(product.ItemId);
	}
}