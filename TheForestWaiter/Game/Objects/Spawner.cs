using SFML.Graphics;
using System;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Environment.Spawning;
using TheForestWaiter.Game.Objects.Abstract;

namespace TheForestWaiter.Game.Objects.Static
{
	internal class Spawner : Immovable
	{

		private SpawnScheduler scheduler;
		private SpawnContext context;
		private ObjectCreator creator;

		public Spawner()
		{
			var content = IoC.GetInstance<ContentSource>();
			scheduler = IoC.GetInstance<SpawnScheduler>();
			creator = IoC.GetInstance<ObjectCreator>();
			context = IoC.GetInstance<SpawnContext>();
		}


		public override void Draw(RenderWindow window)
		{
			
		}

		public override void Update(float time)
		{
			scheduler.Update(time);

			while (scheduler.Jobs.Any())
			{
				var job = scheduler.Jobs.Dequeue();
				var enemy = creator.CreateType(Types.GameObjects[job.EnemyName]);
				enemy.Position = context.GetPosition(job.Side);
				Game.Objects.AddGameObject(enemy);
			}
		}
	}
}
