using System;
using System.IO;
using System.Text.Json;

namespace Engine.Config
{
    /// <summary>
    /// Statischer Zugriff auf die aktuellen Settings + Laden aus settings.json.
    /// </summary>
    public static class Settings
    {
        public static SettingsDTO Current { get; private set; } = new SettingsDTO();

        /// <summary>
        /// Lädt Settings aus der angegebenen Datei. Wenn path null ist,
        /// wird "settings.json" im Ausgabeverzeichnis (AppContext.BaseDirectory) verwendet.
        /// </summary>
        public static void Load(string? path = null)
        {
            string file = path ?? Path.Combine(AppContext.BaseDirectory, "settings.json");
            if (!File.Exists(file))
                throw new FileNotFoundException($"settings.json nicht gefunden: {file}");

            var json = File.ReadAllText(file);

            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var loaded = JsonSerializer.Deserialize<SettingsDTO>(json, opts);
            if (loaded == null)
                throw new InvalidOperationException("settings.json konnte nicht deserialisiert werden.");

            Current = loaded;
        }

        /// <summary>
        /// Hilfsfunktion: Pfad relativ zu ContentRoot auflösen.
        /// </summary>
        public static string ContentPath(string relative)
        {
            var root = Current.ContentRoot?.TrimEnd('/', '\\') ?? "Content";
            return Path.Combine(AppContext.BaseDirectory, root, relative);
        }
    }
}
