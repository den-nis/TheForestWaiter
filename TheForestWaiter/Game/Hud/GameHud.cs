using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using TheForestWaiter.Content;
using TheForestWaiter.Game.Constants;
using TheForestWaiter.Game.Hud.Sections;

namespace TheForestWaiter.Game.Hud
{
    internal class GameHud
    {
        public bool IsCaptured { get; private set; } = false;

        private readonly List<HudSection> _sections;
        private readonly Camera _camera;

        public GameHud(GameData game, Camera camera, ContentSource content, UserSettings settings, ItemShop shop)
        {
            _camera = camera;

            var scale = settings.GetFloat("Game", "HudScale");

            _sections = new()
            {
                new PlayerHud(scale, game, content)
                {
                    Region = HudRegion.TopLeft,
                    Offset = new Vector2f(10, 10),
                },
                new WeaponsHud(scale, camera, game, content)
                {
                    Region = HudRegion.BottomLeft,
                },
                new ShopHud(scale, content, shop, camera)
                {
                    Region = HudRegion.TopRight,
                },
                new WaveHud(scale, game, content)
                {
                    Region = HudRegion.BottomRight,
                    Offset = new Vector2f(-10, -10)
				}
            };
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(Camera.GetWindowView(window));

            foreach (var section in _sections.Where(s => !s.Hidden))
            {
                section.Draw(window);
            }

            window.SetView(_camera.GetView());
        }

        public void PrimaryReleased()
        {
            foreach (var section in _sections.Where(s => !s.Hidden))
            {
                section.OnPrimaryReleased();
            }
        }

        public void PrimaryPressed()
        {
            if (ShouldCaptureWhenPress())
            {
                IsCaptured = true;
                foreach (var section in _sections.Where(s => !s.Hidden))
                {
                    section.OnPrimaryPressed();
                }
            }
            else
            {
                IsCaptured = false;
            }
        }

        public void OnMouseMove(Vector2i mouse)
        {
            foreach (var section in _sections)
            {
                section.OnMouseMove(mouse);
			}
		}

        public void ToggleShopVisibility()
        {
            var section = _sections.First(s => s is ShopHud);
            section.Hidden = !section.Hidden;
        }

        private bool ShouldCaptureWhenPress()
        {
            foreach (var section in _sections.Where(s => !s.Hidden))
            {
                if (section.IsMouseOnAnyButton())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
