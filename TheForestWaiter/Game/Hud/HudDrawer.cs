using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Essentials;
using TheForestWaiter.Game.Graphics;
using TheForestWaiter.Game.Hud.Sections;

namespace TheForestWaiter.Game.Hud
{
    internal class HudDrawer
    {
        public readonly List<HudSection> _sections;
        private readonly Camera _camera;

        public HudDrawer(GameData game, Camera camera, ContentSource content, UserSettings settings)
        {
            _camera = camera;

            var scale = settings.GetFloat("Game", "HudScale");

            _sections = new()
            {
                new PlayerHud(game, content)
                {
                    Region = HudRegion.TopLeft,
                    Offset = new Vector2f(10, 10),
                    Scale = scale
                },
                new WeaponsHud(game, content)
                {
                    Region = HudRegion.BottomLeft,
                    Scale = scale
                }
            };
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(Camera.GetWindowView(window));

            foreach(var section in _sections)
            {
                section.Draw(window);
			}

            window.SetView(_camera.GetView());
        }
    }
}
