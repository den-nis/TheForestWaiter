using System;
using System.Collections.Generic;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Logic
{
	internal class WeaponCollection
	{
	
		/// <summary>
		/// Parameter = previous weaopn
		/// </summary>
		public event Action<Weapon> OnEquipedChanged = delegate { };

		public IReadOnlyList<Weapon> OwnedWeapons => _ownedWeapons;
		private readonly List<Weapon> _ownedWeapons = new();
		private int _equipedIndex = 0;

		public void Add(Weapon weapon)
		{
			_ownedWeapons.Add(weapon);
		}

		public Weapon GetEquiped()
		{
			return _ownedWeapons[_equipedIndex];
		}

		public void Select(int index)
		{
			index = Math.Clamp(index, 0, _ownedWeapons.Count - 1);

			if (index != _equipedIndex)
			{
				var previous = _equipedIndex;
				_equipedIndex = index;
				OnEquipedChanged(_ownedWeapons[previous]);
			}
		}

		public void MoveSelection(int direction)
		{
			var index = _equipedIndex + direction;

			if (index > _ownedWeapons.Count)
				index = 0;

			if (index < 0)
				index = _ownedWeapons.Count - 1;

			Select(index);
		}
	}
}
