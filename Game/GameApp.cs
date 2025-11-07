using Engine.Config;
using Engine.Core;
using Engine.Diagnostics;
using Engine.IO;
using Engine.Content;
using Engine.Resources;
using Engine.Runtime;

namespace Game
{
    public sealed class GameApp : IGameApp
    {
        private ISceneService? _scenes;
        private IEventRunner? _events;

        public void Initialize()
        {
            Settings.Load();

            ServiceRegistry.Clear();

            // Create MultiLogger with Console and File loggers
            var multiLogger = new MultiLogger(LogLevel.Debug);
            
            var consoleLogger = new ConsoleLogger(LogLevel.Debug);
            multiLogger.AddLogger(consoleLogger);
            
            // Add file logger
            var logDir = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Logs");
            var logFile = System.IO.Path.Combine(logDir, $"game_{System.DateTime.Now:yyyyMMdd_HHmmss}.log");
            var fileLogger = new FileLogger(logFile, LogLevel.Debug);
            multiLogger.AddLogger(fileLogger);
            
            ServiceRegistry.Register<ILogger>(multiLogger);
            Log.Use(multiLogger);

            var fs = new FileSystem();
            ServiceRegistry.Register<IFileSystem>(fs);

            var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
            ServiceRegistry.Register<IContentResolver>(resolver);

            var rm = new ResourceManager(resolver, fs, multiLogger);
            rm.RegisterDefaultLoaders();
            ServiceRegistry.Register<IResourceManager>(rm);

            // SceneService + EventRunner registrieren
            _scenes = new SceneService();
            ServiceRegistry.Register<ISceneService>(_scenes);

            _events = new LogEventRunner();
            ServiceRegistry.Register<IEventRunner>(_events);

            // Startszene laden (falls leer: fallback auf example)
            var start = string.IsNullOrWhiteSpace(Settings.Current.StartSceneId)
                        ? "Scenes/first.scene.json"
                        : Settings.Current.StartSceneId; // hier interpretieren wir StartSceneId als Pfad

            _scenes.LoadFromContent(start);

            // Mini-Probe: HitTest bei (x,y) — passe Koordinaten an deine Testszene an
            var probe = _scenes.HitTest(900, 520);
            if (probe != null)
            {
                Log.Info(nameof(GameApp), $"Hit hotspot: {probe.Id}");
                if (!string.IsNullOrWhiteSpace(probe.EventPathId))
                    _events.Trigger(probe.EventPathId!);
            }
            else
            {
                Log.Info(nameof(GameApp), "No hotspot hit at (900,520).");
            }
        }

        public void Update(float dt) { }

        public void Shutdown()
        {
            Log.Info(nameof(GameApp), "Shutdown.");
        }
    }
}
