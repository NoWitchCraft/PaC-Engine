using Engine.Config;
using Engine.Core;
using Engine.Diagnostics;

namespace Game
{
    public sealed class GameApp : IGameApp
    {
        public void Initialize()
        {
            Settings.Load();

            // Services zurücksetzen & Logger registrieren
            ServiceRegistry.Clear();
            var logger = new ConsoleLogger(minLevel: LogLevel.Debug);
            ServiceRegistry.Register<ILogger>(logger);
            Log.Use(logger);

            Log.Info(nameof(GameApp), $"ContentRoot = {Settings.Current.ContentRoot}");
        }

        public void Update(float dt)
        {
            // Beispiel:
            // Log.Debug(nameof(GameApp), $"dt={dt:F4}");
        }

        public void Shutdown()
        {
            Log.Info(nameof(GameApp), "Shutdown.");
        }
    }
}
