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
        public static EventLogger? EventLogger { get; private set; }

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

                // Create MultiLogger with Console, File, and Event loggers
                var multiLogger = new MultiLogger(LogLevel.Debug);
                
                var consoleLogger = new ConsoleLogger(LogLevel.Info);
                multiLogger.AddLogger(consoleLogger);
                
                // Add file logger
                var logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
                var logFile = Path.Combine(logDir, $"editor_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
                var fileLogger = new FileLogger(logFile, LogLevel.Debug);
                multiLogger.AddLogger(fileLogger);
                
                // Add event logger for UI
                EventLogger = new EventLogger(LogLevel.Debug);
                multiLogger.AddLogger(EventLogger);
                
                ServiceRegistry.Register<ILogger>(multiLogger);
                Log.Use(multiLogger);

                var fs = new FileSystem();
                ServiceRegistry.Register<IFileSystem>(fs);

                // ContentResolver expects absolute path to content root
                var contentRootAbsolute = Path.Combine(projectPath, Settings.Current.ContentRoot);
                var resolver = new ContentResolver(fs, contentRootAbsolute);
                ServiceRegistry.Register<IContentResolver>(resolver);

                var rm = new ResourceManager(resolver, fs, multiLogger);
                rm.RegisterDefaultLoaders();
                ServiceRegistry.Register<IResourceManager>(rm);

                Log.Info(nameof(App), $"Editor ContentRootAbs = {resolver.ContentRootAbsolute}");
                Log.Info(nameof(App), $"Project Path = {projectPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Projekt konnte nicht initialisiert werden:\n{ex.Message}",
                    "Editor – Initialisierungsfehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
            }
        }
    }
}
