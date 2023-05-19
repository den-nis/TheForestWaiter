using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter.Game.Logic
{
	internal class WeaponWheelItem 
	{
		public bool IsEmpty => Weapon == null;
		public float Angle { get; set; } = 0;
		public Weapon Weapon { get; set; } = null;
		public bool IsInFocus { get; set; } = false;
	}
}
