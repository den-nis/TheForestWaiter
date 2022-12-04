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
		private static IReadOnlyList<Type> _orderedGameObjects;
		private static IReadOnlyDictionary<Type, ushort> _indexLookup;

		public static IReadOnlyDictionary<string, Type> GameObjects { get; private set; }
		public static IReadOnlyDictionary<string, Type> Weapons { get; private set; }

		static Types()
		{
			GameObjects = GetGameObjects();
			Weapons = GetWeapons();
			SetupIndex(GameObjects.Values);
		}

		public static void SetupIndex(IEnumerable<Type> objects)
		{
			_orderedGameObjects = objects.OrderBy(x => x.Name).ToList();
			_indexLookup = Enumerable.Range(0, _orderedGameObjects.Count).ToDictionary(k => _orderedGameObjects[k], v => (ushort)v);
		}

		public static ushort GetIndexByType(Type type) => _indexLookup[type];

		public static Type GetTypeByIndex(ushort index) => _orderedGameObjects[index];

		private static Dictionary<string, Type> GetGameObjects() => GetTypes<GameObject>();

		private static Dictionary<string, Type> GetWeapons() => GetTypes<Weapon>();

		private static Dictionary<string, Type> GetTypes<T>()
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
