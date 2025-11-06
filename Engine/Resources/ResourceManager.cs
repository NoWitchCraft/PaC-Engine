using System;
using System.Collections.Concurrent;
using Engine.Content;
using Engine.Diagnostics;
using Engine.IO;

namespace Engine.Resources
{
    /// <summary>
    /// Default implementation of IResourceManager that provides thread-safe
    /// resource caching and lazy-loading capabilities.
    /// </summary>
    public sealed class ResourceManager : IResourceManager
    {
        private readonly IContentResolver _contentResolver;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger? _logger;

        // Thread-safe cache: key = normalized relative path, value = loaded resource
        private readonly ConcurrentDictionary<string, object> _cache = new();

        // Type-specific loaders: key = resource type, value = loader function
        private readonly ConcurrentDictionary<Type, Delegate> _loaders = new();

        /// <summary>
        /// Initializes a new instance of the ResourceManager.
        /// </summary>
        /// <param name="contentResolver">Content path resolver for locating assets.</param>
        /// <param name="fileSystem">File system abstraction for reading files.</param>
        /// <param name="logger">Optional logger for debug output.</param>
        public ResourceManager(
            IContentResolver contentResolver,
            IFileSystem fileSystem,
            ILogger? logger = null)
        {
            _contentResolver = contentResolver ?? throw new ArgumentNullException(nameof(contentResolver));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _logger = logger;
        }

        /// <inheritdoc/>
        public T Get<T>(string path) where T : class
        {
            return LoadInternal<T>(path);
        }

        /// <inheritdoc/>
        public T Load<T>(string path) where T : class
        {
            return LoadInternal<T>(path);
        }

        /// <inheritdoc/>
        public bool Unload(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var normalizedPath = NormalizePath(path);
            
            if (_cache.TryRemove(normalizedPath, out var resource))
            {
                _logger?.Debug("ResourceManager", $"Unloaded: {normalizedPath}");
                
                // Dispose if the resource implements IDisposable
                if (resource is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void UnloadAll()
        {
            _logger?.Info("ResourceManager", $"Unloading all resources ({_cache.Count} items)");

            foreach (var kvp in _cache)
            {
                if (kvp.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _cache.Clear();
        }

        /// <inheritdoc/>
        public bool IsLoaded(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            var normalizedPath = NormalizePath(path);
            return _cache.ContainsKey(normalizedPath);
        }

        /// <inheritdoc/>
        public void RegisterLoader<T>(Func<string, T> loader) where T : class
        {
            if (loader == null)
                throw new ArgumentNullException(nameof(loader));

            var type = typeof(T);
            _loaders[type] = loader;

            _logger?.Debug("ResourceManager", $"Registered loader for type: {type.Name}");
        }

        /// <summary>
        /// Internal method to load or retrieve a cached resource.
        /// </summary>
        private T LoadInternal<T>(string path) where T : class
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Resource path cannot be null or empty.", nameof(path));

            var normalizedPath = NormalizePath(path);

            // Try to get from cache first
            if (_cache.TryGetValue(normalizedPath, out var cached))
            {
                if (cached is T typedResource)
                {
                    _logger?.Debug("ResourceManager", $"Cache hit: {normalizedPath}");
                    return typedResource;
                }

                // Type mismatch - throw exception
                throw new InvalidOperationException(
                    $"Resource '{normalizedPath}' is cached as {cached.GetType().Name}, but requested as {typeof(T).Name}");
            }

            // Not in cache - need to load it
            _logger?.Debug("ResourceManager", $"Loading: {normalizedPath}");

            var absolutePath = _contentResolver.ResolveContentPath(normalizedPath);

            // Check if file exists
            if (!_fileSystem.FileExists(absolutePath))
            {
                throw new InvalidOperationException(
                    $"Resource file not found: {normalizedPath} (resolved to: {absolutePath})");
            }

            // Get the loader for this type
            var type = typeof(T);
            if (!_loaders.TryGetValue(type, out var loaderDelegate))
            {
                throw new InvalidOperationException(
                    $"No loader registered for type: {type.Name}. Use RegisterLoader<{type.Name}> to register a loader.");
            }

            // Cast and invoke the loader
            var loader = (Func<string, T>)loaderDelegate;
            T resource;

            try
            {
                resource = loader(absolutePath);
            }
            catch (Exception ex)
            {
                _logger?.Error("ResourceManager", $"Failed to load '{normalizedPath}': {ex.Message}");
                throw new InvalidOperationException(
                    $"Failed to load resource '{normalizedPath}'", ex);
            }

            if (resource == null)
            {
                throw new InvalidOperationException(
                    $"Loader for type {type.Name} returned null for path: {normalizedPath}");
            }

            // Cache the loaded resource
            _cache[normalizedPath] = resource;
            _logger?.Info("ResourceManager", $"Loaded and cached: {normalizedPath}");

            return resource;
        }

        /// <summary>
        /// Normalizes a path by replacing backslashes with forward slashes
        /// and trimming leading slashes.
        /// </summary>
        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimStart('/');
        }
    }
}
