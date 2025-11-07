using System.IO;

namespace Editor
{
    /// <summary>
    /// Helper for creating and managing project-related files.
    /// </summary>
    public static class ProjectHelper
    {
        /// <summary>
        /// Gets the default settings.json content.
        /// </summary>
        public static string GetDefaultSettingsJson()
        {
            return @"{
  ""ContentRoot"": ""Content"",
  ""StartSceneId"": """",
  ""WindowWidth"": 1280,
  ""WindowHeight"": 720,
  ""Language"": ""de-DE"",
  ""MasterVolume"": 1.0
}";
        }

        /// <summary>
        /// Creates a default settings.json file at the specified path.
        /// </summary>
        public static void CreateDefaultSettings(string settingsPath)
        {
            File.WriteAllText(settingsPath, GetDefaultSettingsJson());
        }
    }
}
