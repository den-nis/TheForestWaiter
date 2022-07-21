using System;

namespace TheForestWaiter.States
{
	public interface IState : IDisposable
	{
		void Draw();

		void Update(float time);

		void Load();
	}
}
