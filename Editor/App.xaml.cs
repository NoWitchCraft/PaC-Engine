using System;
using System.Windows;
using Engine.Config;
using Engine.Core;
using Engine.Diagnostics;

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
