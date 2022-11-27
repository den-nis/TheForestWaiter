using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game;

internal class Inventory
{
    private Dictionary<int, Weapon> _weaponInstances = new();

    /// <summary>
    /// Parameter is previous weapon
    /// </summary>
    public event Action<Weapon> OnEquipWeapon;

    /// <summary>
    /// Parameter is previous itemId
    /// </summary>
    public event Action<int> OnEquipedChanged;

    public Creature Owner { get; set; }
    public int EquipedIndex  => _equipedIndex;
    public IEnumerable<Weapon> Weapons => _weaponInstances.Values;
    public IReadOnlyList<int> Items => _items;

    private List<int> _items = new();
    private int _equipedIndex;

	private readonly ItemRepository _repostiory;
	private IServiceContainer _container;

	public Inventory()
    {
		_repostiory = IoC.GetInstance<ItemRepository>();
        _container = IoC.GetInstance<IServiceContainer>();
	}

    public void Overwrite(IEnumerable<int> items)
    {
        _items = items.ToList();
        UpdateWeaponInstances();
    }

    public int GetCurrentItem()
    {
        return _items[_equipedIndex];
    }

    public Weapon GetCurrentWeapon()
    {   
        if (!_items.Any()) return null;

        var itemId = _items[_equipedIndex];
        return (_weaponInstances.ContainsKey(itemId))
            ? _weaponInstances[itemId]
            : null;
    }
    
    public void Add(int id)
    {
        if (Owner == null)
            throw new InvalidOperationException("There is no owner of this inventory");

        _items.Add(id);
        UpdateWeaponInstances();
    }

    private void UpdateWeaponInstances()
    {
        foreach (var item in _items)
        {
            var product = _repostiory.GetItemInfo(item);

            if (product.Type == "Weapon" && !_weaponInstances.ContainsKey(item))
            {
                var weapon =  _container.GetInstance(Types.Weapons[product.Name]) as Weapon;
                weapon.Owner = Owner;
                 _weaponInstances.Add(item, weapon);
            }
        }
    }

    public void Select(int index)
    {
        index = Math.Clamp(index, 0, _items.Count - 1);

        if (index != _equipedIndex)
        {
            var previous = _equipedIndex;
            _equipedIndex = index;

            var product = _repostiory.GetItemInfo(_items[previous]);
            OnEquipedChanged?.Invoke(_items[previous]);

            Weapon previousWeapon = _weaponInstances.ContainsKey(_items[previous])
                ? _weaponInstances[_items[previous]]
                : null;

            if (_weaponInstances.ContainsKey(_items[_equipedIndex]))
                OnEquipWeapon?.Invoke(previousWeapon);
        }
    }

    public void MoveSelection(int direction)
    {
        var index = _equipedIndex + direction;

        if (index > _items.Count)
            index = 0;

        if (index < 0)
            index = _items.Count - 1;

        Select(index);
    }
}