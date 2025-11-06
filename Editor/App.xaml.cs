using Engine.Config;
using Engine.Content;
using Engine.Core;
using Engine.Diagnostics;
using Engine.IO;
using System;
using System.Windows;

namespace Editor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Settings.Load();

                ServiceRegistry.Clear();
                var logger = new ConsoleLogger(minLevel: LogLevel.Info);
                ServiceRegistry.Register<ILogger>(logger);
                Log.Use(logger);

                var fs = new FileSystem();
                ServiceRegistry.Register<IFileSystem>(fs);

                var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
                ServiceRegistry.Register<IContentResolver>(resolver);

                Log.Info(nameof(App), $"Editor ContentRootAbs = {resolver.ContentRootAbsolute}");

                Log.Info(nameof(App), "Editor started.");
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
