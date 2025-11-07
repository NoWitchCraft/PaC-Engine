using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Editor
{
    /// <summary>
    /// Manages recently opened projects, saving and loading from a JSON file.
    /// </summary>
    public static class RecentProjectsManager
    {
        private const int MaxRecentProjects = 10;
        private static readonly string RecentProjectsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PaCEngine",
            "RecentProjects.json"
        );

        /// <summary>
        /// Loads the list of recently opened projects.
        /// </summary>
        public static List<ProjectInfo> LoadRecentProjects()
        {
            try
            {
                if (!File.Exists(RecentProjectsFile))
                    return new List<ProjectInfo>();

                var json = File.ReadAllText(RecentProjectsFile);
                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                var projects = JsonSerializer.Deserialize<List<ProjectInfo>>(json, opts);
                return projects ?? new List<ProjectInfo>();
            }
            catch (JsonException)
            {
                // If JSON is corrupted, return empty list and file will be overwritten on next save
                return new List<ProjectInfo>();
            }
            catch (IOException)
            {
                // If file can't be read, return empty list
                return new List<ProjectInfo>();
            }
            catch (UnauthorizedAccessException)
            {
                // If access is denied, return empty list
                return new List<ProjectInfo>();
            }
        }

        /// <summary>
        /// Saves the list of recently opened projects.
        /// </summary>
        public static void SaveRecentProjects(List<ProjectInfo> projects)
        {
            try
            {
                var directory = Path.GetDirectoryName(RecentProjectsFile);
                if (directory != null && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                // Keep only the most recent projects
                var toSave = projects
                    .OrderByDescending(p => p.LastOpened)
                    .Take(MaxRecentProjects)
                    .ToList();

                var opts = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(toSave, opts);
                File.WriteAllText(RecentProjectsFile, json);
            }
            catch (JsonException)
            {
                // Silently fail if serialization fails
            }
            catch (IOException)
            {
                // Silently fail if we can't write the file
            }
            catch (UnauthorizedAccessException)
            {
                // Silently fail if access is denied
            }
        }

        /// <summary>
        /// Adds a project to the recent projects list.
        /// </summary>
        public static void AddRecentProject(ProjectInfo project)
        {
            var projects = LoadRecentProjects();

            // Remove if already exists
            projects.RemoveAll(p => p.Path.Equals(project.Path, StringComparison.OrdinalIgnoreCase));

            // Add at the beginning
            project.LastOpened = DateTime.Now;
            projects.Insert(0, project);

            SaveRecentProjects(projects);
        }
    }
}
