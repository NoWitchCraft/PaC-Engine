# Resource Manager

The Resource Manager subsystem provides lightweight asset loading, caching, and lazy resource management for the PaC Engine.

## Features

- **Centralized resource loading and caching**: Single point of access for all external assets
- **Generic type-based access**: Load any resource type using `Get<T>(path)` or `Load<T>(path)`
- **Automatic reuse**: Already loaded assets are returned from cache, preventing redundant loads
- **Manual resource management**: Option to unload specific resources or all cached resources
- **Thread-safe**: Uses `ConcurrentDictionary` for safe concurrent access
- **Integration**: Works seamlessly with `IFileSystem`, `IContentResolver`, and `ServiceRegistry`
- **Optional logging**: Debug output via `ILogger` for tracking resource operations

## Quick Start

### 1. Register the Resource Manager

During application initialization, create and register the ResourceManager:

```csharp
using Engine.Core;
using Engine.Resources;
using Engine.Content;
using Engine.IO;
using Engine.Diagnostics;

// Assuming IFileSystem, IContentResolver, and ILogger are already registered
var fs = ServiceRegistry.Get<IFileSystem>();
var resolver = ServiceRegistry.Get<IContentResolver>();
var logger = ServiceRegistry.Get<ILogger>(); // Optional

var resourceManager = new ResourceManager(resolver, fs, logger);
ServiceRegistry.Register<IResourceManager>(resourceManager);
```

### 2. Register Resource Loaders

Register loaders for each resource type you want to load:

```csharp
// Example: Register a simple text file loader
resourceManager.RegisterLoader<string>(absolutePath => 
{
    return File.ReadAllText(absolutePath);
});

// Example: Register a JSON object loader
resourceManager.RegisterLoader<MyDataClass>(absolutePath =>
{
    var json = File.ReadAllText(absolutePath);
    return JsonSerializer.Deserialize<MyDataClass>(json);
});

// Example: Register an image loader (pseudo-code)
resourceManager.RegisterLoader<Image>(absolutePath =>
{
    return Image.LoadFromFile(absolutePath);
});
```

### 3. Load and Use Resources

```csharp
// Get resource manager from service registry
var rm = ServiceRegistry.Get<IResourceManager>();

// Load a resource (or get from cache if already loaded)
var configText = rm.Get<string>("config/settings.txt");
var uiData = rm.Load<MyDataClass>("ui/main_menu.json");
var buttonImage = rm.Get<Image>("ui/button.png");

// Check if a resource is loaded
if (rm.IsLoaded("ui/button.png"))
{
    Console.WriteLine("Button image is cached!");
}

// Unload a specific resource
rm.Unload("ui/button.png");

// Unload all resources (e.g., during level transitions)
rm.UnloadAll();
```

## API Reference

### IResourceManager Interface

#### Methods

- **`T Get<T>(string path)`**  
  Loads or retrieves a cached resource. Returns the resource instance.
  - `path`: Relative content path to the resource

- **`T Load<T>(string path)`**  
  Functionally equivalent to `Get<T>`. Explicitly loads or retrieves a cached resource.
  - `path`: Relative content path to the resource

- **`bool Unload(string path)`**  
  Unloads a specific resource from cache. Returns true if successful.
  - Automatically disposes resources implementing `IDisposable`

- **`void UnloadAll()`**  
  Unloads all cached resources.
  - Disposes all resources implementing `IDisposable`

- **`bool IsLoaded(string path)`**  
  Checks if a resource is currently loaded in cache.

- **`void RegisterLoader<T>(Func<string, T> loader)`**  
  Registers a loader function for a specific resource type.
  - `loader`: Function that receives an absolute file path and returns a resource instance

### ResourceManager Implementation

The `ResourceManager` class is the default implementation of `IResourceManager`.

#### Constructor

```csharp
public ResourceManager(
    IContentResolver contentResolver,
    IFileSystem fileSystem,
    ILogger? logger = null)
```

- `contentResolver`: Required. Used to resolve relative paths to absolute paths
- `fileSystem`: Required. Used to check file existence and read files
- `logger`: Optional. Used for debug/info logging

#### Thread Safety

All public methods are thread-safe and can be called from multiple threads concurrently.

## Integration Example

Here's a complete example showing integration with the existing GameApp initialization:

```csharp
public void Initialize()
{
    Settings.Load();
    ServiceRegistry.Clear();

    // Register core services
    var logger = new ConsoleLogger(minLevel: LogLevel.Debug);
    ServiceRegistry.Register<ILogger>(logger);
    Log.Use(logger);

    var fs = new FileSystem();
    ServiceRegistry.Register<IFileSystem>(fs);

    var resolver = new ContentResolver(fs, Settings.Current.ContentRoot);
    ServiceRegistry.Register<IContentResolver>(resolver);

    // Register Resource Manager
    var resourceManager = new ResourceManager(resolver, fs, logger);
    ServiceRegistry.Register<IResourceManager>(resourceManager);

    // Register loaders for your resource types
    resourceManager.RegisterLoader<string>(path => fs.ReadAllText(path));
    
    // Now you can load resources throughout your application
    var sceneData = resourceManager.Get<string>("Scenes/first.scene.json");
}
```

## Best Practices

1. **Register loaders early**: Register all your resource loaders during application initialization
2. **Use consistent paths**: Always use forward slashes and relative paths from ContentRoot
3. **Manage memory**: Call `UnloadAll()` during level transitions or when switching scenes
4. **Dispose properly**: Implement `IDisposable` on resource types that need cleanup
5. **Handle errors**: Wrap resource loading in try-catch blocks for graceful error handling

## Error Handling

The ResourceManager throws `InvalidOperationException` in these cases:
- No loader is registered for the requested type
- The resource file doesn't exist
- The loader function fails to load the resource
- Type mismatch (resource cached as one type, requested as another)

Example:

```csharp
try
{
    var image = rm.Get<Image>("ui/missing.png");
}
catch (InvalidOperationException ex)
{
    Log.Error("ResourceManager", $"Failed to load image: {ex.Message}");
    // Fall back to default image or handle gracefully
}
```

## Future Enhancements

Potential improvements for future versions:

- **ResourceHandle&lt;T&gt;**: Reference counting for automatic resource management
- **Async loading**: Support for asynchronous resource loading
- **Progress callbacks**: Track loading progress for large resources
- **Resource groups**: Load/unload groups of related resources together
- **Hot reload**: Detect file changes and reload resources automatically
