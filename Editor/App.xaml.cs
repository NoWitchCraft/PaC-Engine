using System;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Settings.Load();

                ServiceRegistry.Clear();

                // Create MultiLogger with Console, File, and Event loggers
                var multiLogger = new MultiLogger(LogLevel.Debug);
                
                var consoleLogger = new ConsoleLogger(LogLevel.Info);
                multiLogger.AddLogger(consoleLogger);
                
                // Add file logger
                var logDir = System.IO.Path.Combine(AppContext.BaseDirectory, "Logs");
                var logFile = System.IO.Path.Combine(logDir, $"editor_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
                var fileLogger = new FileLogger(logFile, LogLevel.Debug);
                multiLogger.AddLogger(fileLogger);
                
                // Add event logger for UI
                EventLogger = new EventLogger(LogLevel.Debug);
                multiLogger.AddLogger(EventLogger);
                
                ServiceRegistry.Register<ILogger>(multiLogger);
                Log.Use(multiLogger);

                var fs = new FileSystem();
                ServiceRegistry.Register<IFileSystem>(fs);

                var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
                ServiceRegistry.Register<IContentResolver>(resolver);

                var rm = new ResourceManager(resolver, fs, multiLogger);
                // In the Editor we avoid registering loaders that force Raylib-cs to load (e.g. textures).
                // Register only basic loaders needed by the editor UI.
                rm.RegisterLoader<string>(abs => fs.ReadAllText(abs));
                rm.RegisterLoader<byte[]>(abs => fs.ReadAllBytes(abs));

                ServiceRegistry.Register<IResourceManager>(rm);

                Log.Info(nameof(App), $"Editor ContentRootAbs = {resolver.ContentRootAbsolute}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Settings konnten nicht geladen werden:\n{ex.Message}",
                    "Editor – Settings Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
            }
        }
    }
}
