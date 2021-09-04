using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.States
{
    public interface IState : IDisposable
    {
        void Draw();

        void Update(float time);

        void Load();
	}
}
