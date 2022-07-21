using TheForestWaiter.Game.Environment;

namespace TheForestWaiter.Game
{
	internal class GameData
	{
		public GameData(World world, GameObjects objects)
		{
			World = world;
			Objects = objects;
		}

		public World World { get; private set; }
		public GameObjects Objects { get; private set; }
		public Session Session { get; private set; } = new();

		public void LoadFromMap(Map map)
		{
			World.LoadFromMap(map);
			Objects.ClearAll();
			Objects.LoadAllFromMap(map);
		}
	}
}
