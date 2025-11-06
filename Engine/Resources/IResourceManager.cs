using System;

namespace Engine.Resources
{
    /// <summary>
    /// Interface for managing resources with caching and lazy-loading capabilities.
    /// Provides centralized access to external assets (images, audio, JSON, etc.)
    /// and prevents redundant loads through automatic caching.
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gets or loads a resource of the specified type from the given path.
        /// If the resource is already cached, returns the cached instance.
        /// Otherwise, loads the resource, caches it, and returns it.
        /// </summary>
        /// <typeparam name="T">The type of resource to retrieve.</typeparam>
        /// <param name="path">The relative content path to the resource.</param>
        /// <returns>The loaded or cached resource instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no loader is registered for the specified type.
        /// </exception>
        T Get<T>(string path) where T : class;

        /// <summary>
        /// Explicitly loads a resource of the specified type from the given path.
        /// If the resource is already cached, returns the cached instance.
        /// Otherwise, loads the resource, caches it, and returns it.
        /// This method is functionally equivalent to Get&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">The type of resource to load.</typeparam>
        /// <param name="path">The relative content path to the resource.</param>
        /// <returns>The loaded or cached resource instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no loader is registered for the specified type.
        /// </exception>
        T Load<T>(string path) where T : class;

        /// <summary>
        /// Unloads a specific resource from the cache.
        /// </summary>
        /// <param name="path">The relative content path to the resource.</param>
        /// <returns>True if the resource was found and unloaded; otherwise, false.</returns>
        bool Unload(string path);

        /// <summary>
        /// Unloads all cached resources.
        /// </summary>
        void UnloadAll();

        /// <summary>
        /// Checks if a resource is currently loaded in the cache.
        /// </summary>
        /// <param name="path">The relative content path to the resource.</param>
        /// <returns>True if the resource is cached; otherwise, false.</returns>
        bool IsLoaded(string path);

        /// <summary>
        /// Registers a loader function for a specific resource type.
        /// The loader function receives the absolute file path and returns an instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of resource the loader handles.</typeparam>
        /// <param name="loader">Function that loads a resource from an absolute file path.</param>
        void RegisterLoader<T>(Func<string, T> loader) where T : class;
    }
}
