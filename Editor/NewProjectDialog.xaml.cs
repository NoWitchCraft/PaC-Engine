using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Editor
{
    public partial class NewProjectDialog : Window
    {
        public string ProjectPath { get; private set; } = "";
        public string ProjectName { get; private set; } = "";

        private string _selectedLocation = "";

        public NewProjectDialog()
        {
            InitializeComponent();
            
            // Set default location to user's Documents folder
            _selectedLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ProjectLocationTextBox.Text = _selectedLocation;
            UpdateFullPath();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Wählen Sie einen Speicherort für Ihr neues Projekt",
                ShowNewFolderButton = true,
                SelectedPath = _selectedLocation
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _selectedLocation = dialog.SelectedPath;
                ProjectLocationTextBox.Text = _selectedLocation;
                UpdateFullPath();
            }
        }

        private void ProjectNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateFullPath();
        }

        private void UpdateFullPath()
        {
            var name = ProjectNameTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(name))
            {
                FullPathTextBlock.Text = "(Projektnamen eingeben)";
                FullPathTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
                CreateButton.IsEnabled = false;
                return;
            }

            if (string.IsNullOrEmpty(_selectedLocation))
            {
                FullPathTextBlock.Text = "(Speicherort wählen)";
                FullPathTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
                CreateButton.IsEnabled = false;
                return;
            }

            // Validate project name (no invalid path characters)
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                FullPathTextBlock.Text = "Ungültiger Projektname (enthält ungültige Zeichen)";
                FullPathTextBlock.Foreground = System.Windows.Media.Brushes.OrangeRed;
                CreateButton.IsEnabled = false;
                return;
            }

            var fullPath = Path.Combine(_selectedLocation, name);
            FullPathTextBlock.Text = fullPath;
            
            if (Directory.Exists(fullPath))
            {
                FullPathTextBlock.Foreground = System.Windows.Media.Brushes.Orange;
                CreateButton.IsEnabled = true; // Allow creating in existing folder
            }
            else
            {
                FullPathTextBlock.Foreground = System.Windows.Media.Brushes.LightGreen;
                CreateButton.IsEnabled = true;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var name = ProjectNameTextBox.Text.Trim();
            var fullPath = Path.Combine(_selectedLocation, name);

            try
            {
                // Create project directory structure
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // Create Content directory
                var contentPath = Path.Combine(fullPath, "Content");
                if (!Directory.Exists(contentPath))
                {
                    Directory.CreateDirectory(contentPath);
                }

                // Create Scenes subdirectory
                var scenesPath = Path.Combine(contentPath, "Scenes");
                if (!Directory.Exists(scenesPath))
                {
                    Directory.CreateDirectory(scenesPath);
                }

                // Create default settings.json
                var settingsPath = Path.Combine(fullPath, "settings.json");
                if (!File.Exists(settingsPath))
                {
                    ProjectHelper.CreateDefaultSettings(settingsPath);
                }

                // Create a README file
                var readmePath = Path.Combine(fullPath, "README.md");
                if (!File.Exists(readmePath))
                {
                    var readme = $@"# {name}

This is a PaC Engine game project.

## Structure
- `Content/` - Game content (scenes, assets, etc.)
- `settings.json` - Project settings

Created on {DateTime.Now:yyyy-MM-dd}
";
                    File.WriteAllText(readmePath, readme);
                }

                ProjectPath = fullPath;
                ProjectName = name;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"Projekt konnte nicht erstellt werden:\n{ex.Message}",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
