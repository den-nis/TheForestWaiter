namespace TheForestWaiter.States
{
	class StateManager
	{
		public bool Loading { get; set; } = false;

		public IState CurrentState { get; set; }

		public void SetState<T>() where T : IState, new() => SetState(new T());

		public void SetState(IState state)
		{
			CurrentState?.Dispose();
			state.Load();
			CurrentState = state;
		}

		public void Draw() => CurrentState.Draw();

		public void Update(float time) => CurrentState.Update(time);
	}
}
