using Engine.Content;
using Engine.Core;
using Engine.Diagnostics;
using Engine.IO;
using Engine.Resources;

namespace Game
{
    public sealed class GameApp : IGameApp
    {
        public void Initialize()
        {
            Settings.Load();

            ServiceRegistry.Clear();

            var logger = new ConsoleLogger(LogLevel.Debug);
            ServiceRegistry.Register<ILogger>(logger);
            Log.Use(logger);

            var fs = new FileSystem();
            ServiceRegistry.Register<IFileSystem>(fs);

            var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
            ServiceRegistry.Register<IContentResolver>(resolver);

            var rm = new ResourceManager(resolver, fs, logger);
            rm.RegisterDefaultLoaders();
            ServiceRegistry.Register<IResourceManager>(rm);

            Log.Info(nameof(GameApp), $"ContentRootAbs = {resolver.ContentRootAbsolute}");
        }

        public void Update(float dt) { }
        public void Shutdown() => Log.Info(nameof(GameApp), "Shutdown.");
    }
}
