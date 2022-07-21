using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheForestWaiter.Game.Objects.Abstract;
using TheForestWaiter.Game.Weapons.Abstract;

namespace TheForestWaiter
{
	internal static class Types
	{
		public static IDictionary<string, Type> GameObjects { get; private set; }
		public static IDictionary<string, Type> Weapons { get; private set; }

		static Types()
		{
			GameObjects = GetGameObjects();
			Weapons = GetWeapons();
		}

		private static IDictionary<string, Type> GetGameObjects() => GetTypes<GameObject>();

		private static IDictionary<string, Type> GetWeapons() => GetTypes<Weapon>();

		private static IDictionary<string, Type> GetTypes<T>()
		{
			var asm = Assembly.GetExecutingAssembly();
			return asm.GetTypes().Where(t =>
				t.IsAssignableTo(typeof(T)) &&
				!t.IsAbstract &&
				!t.IsInterface)
				.ToDictionary(k => k.Name, v => v);
		}
	}
}
