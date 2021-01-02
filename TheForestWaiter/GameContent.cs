using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TheForestWaiter.Debugging;

namespace TheForestWaiter
{
    public static class GameContent
    {
        private static Assembly CurrentAssembly { get; } = Assembly.GetExecutingAssembly();

        private const string DEFAULT_PREFIX = "TheForestWaiter.";

        private static Dictionary<string, Texture> Textures { get; } = new Dictionary<string, Texture>();
        private static Dictionary<string, Font> Fonts { get; } = new Dictionary<string, Font>();
        private const string TEXTURE_FILES_PREFIX = "Content.Textures.";
        private const string FONT_FILES_PREFIX = "Content.Fonts.";

        public static Texture GetTexture(string name) => GetGeneric(name, Textures);
        public static Font GetFont(string name) => GetGeneric(name, Fonts);

        private static T GetGeneric<T>(string name, Dictionary<string, T> dictionary)
        {
            Debug.Assert(dictionary.Count > 0, $"Could not load {typeof(T).Name}, no {typeof(T).Name}s are loaded");
            Debug.Assert(dictionary.ContainsKey(name), $"Could not find texture \"{name}\"");
            return dictionary[name];
        }

        private static string[] GetContentNames(string directory, string extension)
        {
            var f = CurrentAssembly.GetManifestResourceNames();

            return CurrentAssembly.GetManifestResourceNames()
                .Select(n => n.Remove(n.IndexOf(DEFAULT_PREFIX), DEFAULT_PREFIX.Length))
                .Where(n =>
                    n.StartsWith(directory) &&
                    n.EndsWith(extension)
                ).ToArray();
        }

        public static Stream LoadContentStream(string name)
        {
            var stream = CurrentAssembly.GetManifestResourceStream(DEFAULT_PREFIX + name);
            if (stream == null)
                throw new ArgumentException($"Could not locate resource {name}");

            return stream;
        }

        public static void LoadAllContent()
        {
            LoadAllTextures();
            LoadAllFonts();
        }

        private static void LoadAllFonts()
        {
            foreach(var name in GetContentNames(FONT_FILES_PREFIX, ".ttf"))
            {
                GameDebug.Log($"Loading font {name}");
                Fonts.Add(name, new Font(LoadContentStream(name)));
            }
        }

        private static void LoadAllTextures()
        {
            foreach (var name in GetContentNames(TEXTURE_FILES_PREFIX, ".png"))
            {
                GameDebug.Log($"Loading texture {name}");
                Textures.Add(name, new Texture(LoadContentStream(name)));
            }
        }
    }
}
