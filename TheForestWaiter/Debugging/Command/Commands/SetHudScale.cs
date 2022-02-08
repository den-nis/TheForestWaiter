using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Game.Hud;

namespace TheForestWaiter.Debugging.Command.Commands
{
    [Command("sethudscale", "", "{number}")]
    class SetHudScale : ICommand
    {
        private readonly HudDrawer _hud;

        public SetHudScale(HudDrawer hud)
        {
            _hud = hud;
        }

        public void Execute(CommandHandler handler, string[] args)
        {
            _hud.HudScale = float.Parse(args[0]);
        }
    }
}
