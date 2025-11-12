using Engine.Config;
using Engine.Core;
using Engine.Diagnostics;
using Engine.IO;
using Engine.Content;
using Engine.Resources;
using Engine.Runtime;
using Engine.Input;
using Engine;

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

            // Logger: Console + File via MultiLogger
            var multi = new MultiLogger(LogLevel.Debug);
            multi.AddLogger(new ConsoleLogger(LogLevel.Debug));

            var logDir = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Logs");
            var logFile = System.IO.Path.Combine(logDir, $"game_{System.DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
            multi.AddLogger(new FileLogger(logFile, LogLevel.Debug));

            ServiceRegistry.Register<ILogger>(multi);
            Log.Use(multi);

            // IO + Content
            var fs = new FileSystem();
            ServiceRegistry.Register<IFileSystem>(fs);

            var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
            ServiceRegistry.Register<IContentResolver>(resolver);

            // ResourceManager (Default-Loader später um Texture ergänzen)
            var rm = new ResourceManager(resolver, fs, multi);
            rm.RegisterDefaultLoaders();
            ServiceRegistry.Register<IResourceManager>(rm);

            // Scene + Events + Input
            _scenes = new SceneService();
            ServiceRegistry.Register<ISceneService>(_scenes);

            _events = new LogEventRunner();
            ServiceRegistry.Register<IEventRunner>(_events);

            IInputService input = new RaylibInputService();
            ServiceRegistry.Register<IInputService>(input);

            // Startszene laden
            var start = string.IsNullOrWhiteSpace(Settings.Current.StartSceneId)
                ? "Scenes/first.scene.json"
                : Settings.Current.StartSceneId;

            _scenes.LoadFromContent(start);

            Log.Info(nameof(GameApp), $"Initialized. StartScene = {start}");
        }

        public void Update(float dt)
        {
            var input = ServiceRegistry.Get<IInputService>();
            if (input.WasPressed(Keys.MouseLeft))
            {
                HitTestHelper.LogHitUnderMouse();
            }
        }

        public void Shutdown()
        {
            Log.Info(nameof(GameApp), "Shutdown.");
        }
    }
}
