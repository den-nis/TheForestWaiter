using TheForestWaiter.UI.Abstract;

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
