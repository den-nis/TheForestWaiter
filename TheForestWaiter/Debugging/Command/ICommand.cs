using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.Command
{
	interface ICommand
	{
		void Execute(string[] args);
	}
}
