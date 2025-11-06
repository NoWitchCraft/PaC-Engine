using System;
using System.Text.Json;
using Engine.Config;
using Engine.Content;
using Engine.Core;
using Engine.Diagnostics;
using Engine.IO;
using Engine.Resources;

namespace Engine.Examples
{
    /// <summary>
    /// Example demonstrating Resource Manager usage.
    /// This example shows how to set up and use the ResourceManager
    /// for loading and caching various types of resources.
    /// </summary>
    public static class ResourceManagerExample
    {
        /// <summary>
        /// Example data class for demonstrating JSON resource loading.
        /// </summary>
        public class GameConfig
        {
            public string? Title { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        /// <summary>
        /// Demonstrates basic Resource Manager setup and usage.
        /// </summary>
        public static void RunExample()
        {
            Console.WriteLine("=== Resource Manager Example ===\n");

            // Step 1: Initialize services (normally done in GameApp.Initialize)
            InitializeServices();

            // Step 2: Get the resource manager from ServiceRegistry
            var resourceManager = ServiceRegistry.Get<IResourceManager>();

            // Step 3: Register resource loaders for different types
            RegisterLoaders(resourceManager);

            // Step 4: Load and use resources
            DemonstrateResourceLoading(resourceManager);

            // Step 5: Demonstrate cache behavior
            DemonstrateCaching(resourceManager);

            // Step 6: Demonstrate resource unloading
            DemonstrateUnloading(resourceManager);

            Console.WriteLine("\n=== Example Complete ===");
        }

        private static void InitializeServices()
        {
            Console.WriteLine("1. Initializing services...");
            
            ServiceRegistry.Clear();

            // Register Logger
            var logger = new ConsoleLogger(minLevel: LogLevel.Debug);
            ServiceRegistry.Register<ILogger>(logger);
            Log.Use(logger);

            // Register FileSystem
            var fs = new FileSystem();
            ServiceRegistry.Register<IFileSystem>(fs);

            // Register ContentResolver
            var resolver = new ContentResolver(fs, "Content");
            ServiceRegistry.Register<IContentResolver>(resolver);

            // Register ResourceManager
            var resourceManager = new ResourceManager(resolver, fs, logger);
            ServiceRegistry.Register<IResourceManager>(resourceManager);

            Console.WriteLine("   Services initialized.\n");
        }

        private static void RegisterLoaders(IResourceManager rm)
        {
            Console.WriteLine("2. Registering resource loaders...");

            // Register loader for text files
            rm.RegisterLoader<string>(absolutePath =>
            {
                Console.WriteLine($"   [Loader] Loading text from: {absolutePath}");
                var fs = ServiceRegistry.Get<IFileSystem>();
                return fs.ReadAllText(absolutePath);
            });

            // Register loader for JSON config files
            rm.RegisterLoader<GameConfig>(absolutePath =>
            {
                Console.WriteLine($"   [Loader] Loading GameConfig from: {absolutePath}");
                var fs = ServiceRegistry.Get<IFileSystem>();
                var json = fs.ReadAllText(absolutePath);
                return JsonSerializer.Deserialize<GameConfig>(json) 
                    ?? throw new InvalidOperationException("Failed to deserialize GameConfig");
            });

            // Register loader for binary data
            rm.RegisterLoader<byte[]>(absolutePath =>
            {
                Console.WriteLine($"   [Loader] Loading binary data from: {absolutePath}");
                var fs = ServiceRegistry.Get<IFileSystem>();
                return fs.ReadAllBytes(absolutePath);
            });

            Console.WriteLine("   Loaders registered.\n");
        }

        private static void DemonstrateResourceLoading(IResourceManager rm)
        {
            Console.WriteLine("3. Loading resources...");

            try
            {
                // Example: Load a text file
                // Note: This will fail if the file doesn't exist - that's expected
                // In a real application, you would have actual content files
                Console.WriteLine("   Attempting to load text file...");
                // var readme = rm.Get<string>("README.txt");
                // Console.WriteLine($"   Loaded: {readme.Substring(0, Math.Min(50, readme.Length))}...");
                Console.WriteLine("   (Skipped - file may not exist in this example)");

                // Example: Load a config file
                // var config = rm.Load<GameConfig>("config/game.json");
                // Console.WriteLine($"   Game Title: {config.Title}");
                Console.WriteLine("   (Skipped - file may not exist in this example)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Expected error (no content files): {ex.Message}");
            }

            Console.WriteLine();
        }

        private static void DemonstrateCaching(IResourceManager rm)
        {
            Console.WriteLine("4. Demonstrating cache behavior...");

            // Create a test file in memory for demonstration
            var testPath = "test/cached.txt";
            
            Console.WriteLine($"   First load of '{testPath}' - will invoke loader");
            Console.WriteLine($"   Is loaded before first Get? {rm.IsLoaded(testPath)}");

            // Note: This example shows the API, but will fail without actual files
            // In production, the second call would use cached version
            Console.WriteLine($"   Second load of '{testPath}' - should use cache");
            Console.WriteLine("   (Would skip loader and return cached instance)\n");
        }

        private static void DemonstrateUnloading(IResourceManager rm)
        {
            Console.WriteLine("5. Demonstrating resource unloading...");

            var testPath = "test/example.txt";

            // Check if loaded
            Console.WriteLine($"   Is '{testPath}' loaded? {rm.IsLoaded(testPath)}");

            // Unload specific resource
            bool unloaded = rm.Unload(testPath);
            Console.WriteLine($"   Unload '{testPath}': {unloaded}");

            // Unload all resources
            Console.WriteLine("   Unloading all cached resources...");
            rm.UnloadAll();
            Console.WriteLine("   All resources unloaded.");
        }

        /// <summary>
        /// Alternative example showing usage in a more realistic scenario.
        /// </summary>
        public static void GameInitializationExample()
        {
            // This shows how you might use ResourceManager in a real game initialization

            // Assuming services are already initialized...
            var rm = ServiceRegistry.Get<IResourceManager>();

            // Register loaders once during startup
            rm.RegisterLoader<string>(path => 
                ServiceRegistry.Get<IFileSystem>().ReadAllText(path));

            // Then throughout your game, load resources as needed:
            // var menuBg = rm.Get<Image>("ui/menu_background.png");
            // var buttonSprite = rm.Get<Image>("ui/button.png");
            // var soundEffect = rm.Get<AudioClip>("audio/click.wav");

            // Resources are automatically cached, so repeated calls are fast:
            // var sameBg = rm.Get<Image>("ui/menu_background.png"); // Returns cached instance

            // When changing levels, unload old resources:
            // rm.Unload("level1/background.png");
            // Or unload everything:
            // rm.UnloadAll();
        }
    }
}
