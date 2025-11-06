using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Engine.Config;
using Engine.Core;
using Engine.IO;
using Engine.Content;
using Engine.Common;

namespace Engine.Data.Scene
{
    /// <summary>
    /// Hilfsfunktion zum Laden/Speichern von SceneDTOs im JSON-Format
    /// </summary>
    public static class  SceneIO
    {
        public static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Läd eine Szene aus einer absoluten Pfad-Datei
        /// </summary>
        public static SceneDTO Load(string absoluteFilePath)
        {
            var fs = ServiceRegistry.Get<IFileSystem>();
            if (!fs.FileExists(absoluteFilePath))
                throw new FileNotFoundException($"Scene-Datei nicht gefunden: {absoluteFilePath}");

            string json = fs.ReadAllText(absoluteFilePath);
            var dto = JsonSerializer.Deserialize<SceneDTO>(json, _jsonOptions);
            if (dto == null)
                throw new InvalidOperationException($"Scene konnte nicht gelesen werden: {absoluteFilePath}");

            return dto;
        }

        /// <summary>
        /// Speichert eine Szene an einem absoluten Pfad
        /// </summary>
        public static void Save(SceneDTO scene, string absoluteFilePath)
        {
            var fs = ServiceRegistry.Get<IFileSystem>();
            fs.CreateDirectory(fs.GetDirectoryName(absoluteFilePath));
            string json = JsonSerializer.Serialize(scene, _jsonOptions);
            fs.WriteAllText(absoluteFilePath, json);
        }

        /// <summary>
        /// Lädt eine Szene relativ zum ContentRoot aus settings.json
        /// </summary>
        public static SceneDTO LoadFromContent(string relativePath)
        {
            var resolver = ServiceRegistry.Get<IContentResolver>();
            var full = resolver.ResolveContentPath(relativePath);
            return Load(full);
        }

        /// <summary>
        /// Speichert eine Szene relativ zum ContentRoot aus settings.json
        /// </summary>
        public static void SaveToContent(SceneDTO scene, string relativePath)
        {
            var resolver = ServiceRegistry.Get<IContentResolver>();
            var full = resolver.ResolveContentPath(relativePath);
            Save(scene, full);
        }
    }
}