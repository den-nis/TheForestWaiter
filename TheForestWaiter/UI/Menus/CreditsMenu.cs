using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.UI.Menus
{
	internal class CreditsMenu : UIState
	{
		public CreditsMenu()
		{
			Controls.Add(new CreditsText());
		}
	}
}
