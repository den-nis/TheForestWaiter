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
    internal class GameHud
    {
        private readonly List<HudSection> _sections;
        private readonly Camera _camera;

        public GameHud(GameData game, Camera camera, ContentSource content, UserSettings settings, ItemShop shop)
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
                },
                new ShopHud(content, shop)
                {
                    Region = HudRegion.TopRight,
                    Scale = scale * 0.9f,
                }
            };
        }

        public void PrimaryReleased()
        {

		}

        public bool IsMouseCaptured()
        {
            bool captured = false;
            foreach (var section in _sections)
            {
                if (section.IsMouseCaptured())
                {
                    captured = true;
				}
            }
            return captured;
		}

        public void Hover(Vector2f mouse)
        {
            foreach (var section in _sections)
            {
                section.Hover(mouse);
			}
		}

        public void ToggleShopVisibility()
        {
            var section = _sections.First(s => s is ShopHud);
            section.Hidden = !section.Hidden;
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(Camera.GetWindowView(window));

            foreach(var section in _sections.Where(s => !s.Hidden))
            {
                section.Draw(window);
			}

            window.SetView(_camera.GetView());
        }
    }
}
