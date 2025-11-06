using Engine;
using Engine.Config;
using Engine.Core;
using Engine.Data.Scene;
using Engine.Diagnostics;

namespace Game
{
    /// <summary>
    /// Zentrale Spieleeinstigsklasse. 
    /// Scenemanagement, Input, Rendering, Audio etc.
    /// </summary>

    public sealed class GameApp : IGameApp
    {
        // Später: Services (SceneManager, AssetLoader, Input Audio, etc.)

        public void Initialize()
        {
            // TODO: Content-Pfade setzen, Services initialisieren, erste Scene vorbereiten

            //Load Settings
            Settings.Load(); // settings.json laden

            //Load Services
            ServiceRegistry.Clear();
            var logger = new ConsoleLogger(minLevel: LogLevel.Debug);
            ServiceRegistry.Register<ILogger>(logger);
            Log.Use(logger);

            //logging
            Log.Info(nameof(GameApp), $"ContentRoot = {Settings.Current.ContentRoot}");

            var scene = SceneIO.LoadFromContent(@"Scenes/first.scene.json");
            Log.Info(nameof(GameApp), $"Scene loaded: {scene.Id}, BG={scene.BackgroundPath}, Hotspots={scene.Hotspots.Count}");

        }

        public void Update(float dt)
        {
            // TODO: Scene updaten, Eingaben verarbeiten, Events ablaufen lassen
        }

        public void Shutdown()
        {
            Log.Info(nameof(GameApp), "Shutdown.");
            //TODO: Ressourcen freigeben, Save/Load flushen
        }
    }
}