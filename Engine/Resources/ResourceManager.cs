using System;
using System.Collections.Concurrent;
using Engine.Content;
using Engine.Diagnostics;
using Engine.IO;

namespace Engine.Resources
{
    public sealed class ResourceManager : IResourceManager
    {
        private readonly IContentResolver _resolver;
        private readonly IFileSystem _fs;
        private readonly ILogger? _log;

        // absoluter Pfad → Lazy<object>; Lazy verhindert Doppel-Laden bei Parallelzugriff
        private readonly ConcurrentDictionary<string, Lazy<object>> _cache = new();
        private readonly ConcurrentDictionary<Type, Delegate> _loaders = new();

        public ResourceManager(IContentResolver resolver, IFileSystem fs, ILogger? log = null)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _fs = fs ?? throw new ArgumentNullException(nameof(fs));
            _log = log;
        }

        public void RegisterLoader<T>(Func<string, T> loader) where T : class
        {
            if (loader is null) throw new ArgumentNullException(nameof(loader));
            _loaders[typeof(T)] = loader;
            _log?.Debug(nameof(ResourceManager), $"Loader registered for {typeof(T).Name}");
        }

        public void RegisterDefaultLoaders()
        {
            RegisterLoader<string>(abs => _fs.ReadAllText(abs));
            RegisterLoader<byte[]>(abs => _fs.ReadAllBytes(abs));
            RegisterLoader<Engine.Rendering.RlTexture>(abs => {
                var tex = Raylib_cs.Raylib.LoadTexture(abs);
                return new Engine.Rendering.RlTexture(abs);
            });
            // Weitere Default-Loader kann man hier nachrüsten (z. B. JsonDocument, Bitmap, etc.)
        }

        public T Get<T>(string relativePath) where T : class
        {
            if (!TryGet<T>(relativePath, out var res))
                throw new InvalidOperationException($"Could not load resource '{relativePath}' as {typeof(T).Name}.");
            return res!;
        }

        public bool TryGet<T>(string relativePath, out T? resource) where T : class
        {
            resource = null;
            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            // 1) Auflösen auf absoluten Pfad → dieser ist der Cache-Key
            var pretty = NormalizeForLog(relativePath);
            var abs = _resolver.ResolveContentPath(pretty);

            // 2) Loader besorgen
            if (!_loaders.TryGetValue(typeof(T), out var del))
            {
                _log?.Error(nameof(ResourceManager), $"No loader for {typeof(T).Name}");
                return false;
            }
            var loader = (Func<string, T>)del;

            // 3) Lazy im Cache hinterlegen/nehmen
            var lazy = _cache.GetOrAdd(abs, _ => new Lazy<object>(() =>
            {
                if (!_fs.FileExists(abs))
                    throw new InvalidOperationException($"File not found: '{pretty}' (resolved: {abs})");

                _log?.Debug(nameof(ResourceManager), $"Loading: {pretty}");
                var obj = loader(abs) ?? throw new InvalidOperationException($"Loader returned null for '{pretty}'");
                _log?.Info(nameof(ResourceManager), $"Loaded: {pretty}");
                return obj;
            }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication));

            try
            {
                var obj = lazy.Value; // lädt einmalig oder liefert aus Cache
                if (obj is T typed)
                {
                    // Bei wiederholtem Zugriff zusätzlich „Cache hit“ loggen
                    if (lazy.IsValueCreated) _log?.Debug(nameof(ResourceManager), $"Cache hit: {pretty}");
                    resource = typed;
                    return true;
                }

                _log?.Error(nameof(ResourceManager),
                    $"Type mismatch for '{pretty}': cached {obj?.GetType().Name ?? "null"} vs requested {typeof(T).Name}");
                return false;
            }
            catch (Exception ex)
            {
                // Bei fehlgeschlagener Erstladung Eintrag entfernen, damit künftige Versuche funktionieren
                _cache.TryRemove(abs, out _);
                _log?.Error(nameof(ResourceManager), $"Load failed '{pretty}': {ex.Message}");
                return false;
            }
        }

        public bool Unload(string relativePath)
        {
            var pretty = NormalizeForLog(relativePath);
            var abs = _resolver.ResolveContentPath(pretty);

            if (_cache.TryRemove(abs, out var lazy))
            {
                if (lazy.IsValueCreated && lazy.Value is IDisposable d)
                    d.Dispose();

                _log?.Debug(nameof(ResourceManager), $"Unloaded: {pretty}");
                return true;
            }
            return false;
        }

        public void UnloadAll()
        {
            _log?.Info(nameof(ResourceManager), $"Unloading all resources ({_cache.Count} items)");
            foreach (var kv in _cache)
            {
                if (_cache.TryRemove(kv.Key, out var lazy))
                    if (lazy.IsValueCreated && lazy.Value is IDisposable d) d.Dispose();
            }
        }

        public bool IsLoaded(string relativePath)
        {
            var pretty = NormalizeForLog(relativePath);
            var abs = _resolver.ResolveContentPath(pretty);
            return _cache.ContainsKey(abs);
        }

        private static string NormalizeForLog(string p)
            => p.Replace('\\', '/').TrimStart('/');
    }
}
