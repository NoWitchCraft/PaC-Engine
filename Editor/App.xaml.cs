using System;
using System.IO;
using System.Windows;
using Engine.Config;
using Engine.Content;
using Engine.Core;
using Engine.Diagnostics;
using Engine.IO;
using Engine.Resources;

namespace Editor
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Show the Start Hub
            var startHub = new StartHub();
            var result = startHub.ShowDialog();

            if (result == true && !string.IsNullOrEmpty(startHub.SelectedProjectPath))
            {
                // Initialize the engine with the selected project
                InitializeEngine(startHub.SelectedProjectPath);

                // Open the main editor window
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // User cancelled or closed the hub - exit application
                Shutdown();
            }
        }

        private void InitializeEngine(string projectPath)
        {
            try
            {
                // Load settings from the selected project path
                var settingsPath = Path.Combine(projectPath, "settings.json");
                Settings.Load(settingsPath);

                ServiceRegistry.Clear();

                var logger = new ConsoleLogger(LogLevel.Info);
                ServiceRegistry.Register<ILogger>(logger);
                Log.Use(logger);

                var fs = new FileSystem();
                ServiceRegistry.Register<IFileSystem>(fs);

                // ContentResolver expects absolute path to content root
                var contentRootAbsolute = Path.Combine(projectPath, Settings.Current.ContentRoot);
                var resolver = new ContentResolver(fs, contentRootAbsolute);
                ServiceRegistry.Register<IContentResolver>(resolver);

                var rm = new ResourceManager(resolver, fs, logger);
                rm.RegisterDefaultLoaders();
                ServiceRegistry.Register<IResourceManager>(rm);

                Log.Info(nameof(App), $"Editor ContentRootAbs = {resolver.ContentRootAbsolute}");
                Log.Info(nameof(App), $"Project Path = {projectPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not initialize project:\n{ex.Message}",
                    "Editor – Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
            }
        }
    }
}
