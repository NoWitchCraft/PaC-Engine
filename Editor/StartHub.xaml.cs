using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Editor
{
    public partial class StartHub : Window
    {
        public string? SelectedProjectPath { get; private set; }

        public StartHub()
        {
            InitializeComponent();
            LoadRecentProjects();
        }

        private void LoadRecentProjects()
        {
            var recentProjects = RecentProjectsManager.LoadRecentProjects();
            
            // Filter out projects that no longer exist
            var validProjects = new List<ProjectInfo>();
            foreach (var project in recentProjects)
            {
                if (Directory.Exists(project.Path))
                {
                    validProjects.Add(project);
                }
            }

            if (validProjects.Count > 0)
            {
                RecentProjectsList.ItemsSource = validProjects;
                NoRecentProjectsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoRecentProjectsText.Visibility = Visibility.Visible;
            }

            // Save the filtered list
            if (validProjects.Count != recentProjects.Count)
            {
                RecentProjectsManager.SaveRecentProjects(validProjects);
            }
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewProjectDialog();
            if (dialog.ShowDialog() == true && !string.IsNullOrEmpty(dialog.ProjectPath))
            {
                OpenProject(dialog.ProjectPath, dialog.ProjectName);
            }
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open Game Project (select settings.json)",
                Filter = "Settings file (settings.json)|settings.json|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var projectPath = Path.GetDirectoryName(dlg.FileName);
                    if (string.IsNullOrEmpty(projectPath))
                    {
                        MessageBox.Show(this, "Invalid project path.", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var projectName = new DirectoryInfo(projectPath).Name;
                    OpenProject(projectPath, projectName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Could not open project:\n{ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RecentProjectsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (RecentProjectsList.SelectedItem is ProjectInfo project)
            {
                if (!Directory.Exists(project.Path))
                {
                    MessageBox.Show(this, 
                        $"Project path no longer exists:\n{project.Path}", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                    LoadRecentProjects(); // Refresh the list
                    return;
                }

                OpenProject(project.Path, project.Name);
            }
        }

        private void OpenProject(string projectPath, string projectName)
        {
            try
            {
                // Validate that the project path contains a settings.json file
                var settingsPath = Path.Combine(projectPath, "settings.json");
                if (!File.Exists(settingsPath))
                {
                    var result = MessageBox.Show(this,
                        $"The selected path does not contain a settings.json file.\n\nPath: {projectPath}\n\nWould you like to create a new settings.json file?",
                        "Missing settings.json",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        CreateDefaultSettings(settingsPath);
                    }
                    else
                    {
                        return;
                    }
                }

                // Add to recent projects
                var projectInfo = new ProjectInfo
                {
                    Name = projectName,
                    Path = projectPath,
                    LastOpened = DateTime.Now
                };
                RecentProjectsManager.AddRecentProject(projectInfo);

                // Set the selected project path and close the hub
                SelectedProjectPath = projectPath;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, 
                    $"Error opening project:\n{ex.Message}", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void CreateDefaultSettings(string settingsPath)
        {
            try
            {
                ProjectHelper.CreateDefaultSettings(settingsPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"Could not create settings.json:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
