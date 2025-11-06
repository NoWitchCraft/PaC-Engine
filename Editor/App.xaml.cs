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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Settings.Load();

                ServiceRegistry.Clear();

                var logger = new ConsoleLogger(LogLevel.Info);
                ServiceRegistry.Register<ILogger>(logger);
                Log.Use(logger);

                var fs = new FileSystem();
                ServiceRegistry.Register<IFileSystem>(fs);

                var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
                ServiceRegistry.Register<IContentResolver>(resolver);

                var rm = new ResourceManager(resolver, fs, logger);
                rm.RegisterDefaultLoaders();
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
