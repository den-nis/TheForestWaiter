using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Game.Weapons.Abstract;
using TheForestWaiter.Game.Essentials;

namespace TheForestWaiter.Game.Logic
{
	internal partial class WeaponWheel
	{
		private const int SLOTS = 8;

		public enum Position 
		{
			Center,
			Mouse,
		}

		/// <summary>
		/// Parameter = previous weaopn
		/// </summary>
		public event Action<Weapon> OnEquipedChanged = delegate { };

		//TODO: make this configurable in Settings.ini
		public Position AnchorType { get; set; } = Position.Mouse;
		public IEnumerable<Weapon> OwnedWeapons => items.Where(x => x.Weapon != null).Select(x => x.Weapon);
		public IEnumerable<WeaponWheelItem> Items => items;
		public Weapon Equiped => items[equipedIndex].Weapon;
		public bool IsSelecting { get; set; } = false;

		private readonly WeaponWheelItem[] items = new WeaponWheelItem[SLOTS]; 
		private int equipedIndex = 0;
		private Vector2i mouseStartPosition;
		private Vector2i mousePosition;
		private readonly Camera camera;
		private readonly float hudScale;
		private readonly float radius;

		public WeaponWheel()
		{
			camera = IoC.GetInstance<Camera>();	
			var settings = IoC.GetInstance<UserSettings>();
			hudScale = settings.GetFloat("Game", "HudScale");
			radius = settings.GetFloat("Game", "WeaponWheelRadius");
			AnchorType = Enum.Parse<Position>(settings.Get("Game", "WeaponWheelAnchor"));

			for (int i = 0; i < SLOTS; i++)
			{
				items[i] = new WeaponWheelItem();
				items[i].Angle = TrigHelper.NormalizeRadians((i * (((float)Math.PI * 2f) / SLOTS)) - ((float)Math.PI / 2f));
			}
		}

		public void Add(Weapon weapon)
		{
			if (items[weapon.Slot].IsEmpty)
			{
				items[weapon.Slot].Weapon = weapon;
			}

			if (Equiped == null)
			{
				equipedIndex = weapon.Slot;
			}
		}

		public void OnMouseMove(Vector2i mousePosition)
		{
			this.mousePosition = mousePosition;

			if (IsSelecting)
			{
				TrySetFocused();
			}
		}

		public void StartSelecting()
		{
			if (!IsSelecting)
			{
				TrySetFocused();
				mouseStartPosition = mousePosition;
			}
			
			IsSelecting = true;
		}

		private void TrySetFocused()
		{
			foreach (var i in items) i.IsInFocus = false;
			var item = FindFocused(); 
			if (item != null) item.IsInFocus = true;
		}

		public void StopSelecting()
		{
			if (IsSelecting)
			{
				var item = FindFocused();
				if (item != null)
				{
					SetEquipment(item);
				}
			}

			IsSelecting = false;
		}

		private WeaponWheelItem FindFocused()
		{
			Vector2i originPosition = GetAnchor();
			Vector2f delta = (mousePosition - originPosition).ToVector2f();

			if (delta.Len() <= radius * 0.25f * hudScale)
			{
				return null;
			}

			float angle = TrigHelper.NormalizeRadians(delta.Angle());
			int nearestIndex = -1;
			float nearest = float.MaxValue;

			for (int i = 0; i < items.Length; i++)
			{
				float distance = Math.Abs(angle - items[i].Angle);
				if (distance < nearest)
				{
					nearestIndex = i;
					nearest = distance; 
				}
			}

			if (nearestIndex != -1) 
			{
				return items[nearestIndex];
			}

			throw new InvalidOperationException("Could not find nearest selected weapon");
		}

		public Vector2i GetAnchor() 
		{
			return AnchorType == Position.Center 
				? new Vector2i((int)camera.ViewportSize.X, (int)camera.ViewportSize.Y) / 2 
				: mouseStartPosition;
		}

		public void Select(int index)
		{
			index = Math.Clamp(index, 0, items.Length - 1);
			if (items[index].Weapon != Equiped)
			{
				SetEquipment(items[index]);
			}
		}

		private void SetEquipment(WeaponWheelItem item)
		{
			var previous = item.Weapon;
			equipedIndex = Array.IndexOf(items, item);
			OnEquipedChanged(previous);
		}

		public void MoveSelection(int direction)
		{
			var index = equipedIndex + direction;

			if (index > items.Length)
				index = 0;

			if (index < 0)
				index = items.Length - 1;

			Select(index);
		}
	}
}
