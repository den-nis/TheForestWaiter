using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Entites;

namespace TheForestWaiter.States
{
    class StateManager : IUpdateDraw
    {
        public bool Loading { get; set; } = false;

        public IState CurrentState { get; set; }

        public void SetState<T>() where T : IState, new() => SetState(new T());
        
        public void SetState(IState state)
        {
            state.Load();
            CurrentState = state;
        }

        public void Draw(RenderWindow window) => CurrentState.Draw();

        public void Update(float time) => CurrentState.Update(time);
        
    }
}
