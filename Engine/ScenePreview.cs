using System;
using Engine.Core;
using Engine.Diagnostics;
using Engine.Rendering;
using Engine.Runtime;

namespace Engine
{
    /// <summary>
    /// Scene Preview Modus für Editor-Integration.
    /// Kann eine Szene ohne vollständiges GameApp laden und anzeigen.
    /// </summary>
    public sealed class ScenePreview : IGameApp
    {
        private readonly string _scenePath;
        private ISceneService? _sceneService;
        private IRenderer? _renderer;
        private bool _sceneLoaded;

        /// <summary>
        /// Erstellt eine neue Scene Preview für den angegebenen Szenen-Pfad
        /// </summary>
        /// <param name="scenePath">Relativer Pfad zur Szene (z.B. "Scenes/first.scene.json")</param>
        public ScenePreview(string scenePath)
        {
            if (string.IsNullOrWhiteSpace(scenePath))
                throw new ArgumentException("Scene path cannot be empty", nameof(scenePath));
            
            _scenePath = scenePath;
        }

        public void Initialize()
        {
            var logger = ServiceRegistry.Get<ILogger>();
            logger?.Info(nameof(ScenePreview), $"Initializing Scene Preview for: {_scenePath}");

            // SceneService aus Registry holen oder erstellen
            _sceneService = ServiceRegistry.TryGet<ISceneService>();
            if (_sceneService == null)
            {
                _sceneService = new SceneService();
                ServiceRegistry.Register<ISceneService>(_sceneService);
            }

            // Renderer aus Registry holen oder erstellen
            _renderer = ServiceRegistry.TryGet<IRenderer>();
            if (_renderer == null)
            {
                _renderer = new ConsoleRenderer(logger);
                _renderer.Initialize();
                ServiceRegistry.Register<IRenderer>(_renderer);
            }

            // Szene laden
            try
            {
                _sceneService.LoadFromContent(_scenePath);
                _sceneLoaded = true;
                logger?.Info(nameof(ScenePreview), $"Scene preview ready: {_sceneService.Current?.Id}");
            }
            catch (Exception ex)
            {
                logger?.Error(nameof(ScenePreview), $"Failed to load scene for preview: {ex.Message}");
                _sceneLoaded = false;
            }
        }

        public void Update(float dt)
        {
            // Preview ist statisch, keine Updates notwendig
            // Könnte später für Animationen, etc. erweitert werden
        }

        public void Render()
        {
            if (!_sceneLoaded || _sceneService == null || _renderer == null)
                return;

            _renderer.BeginFrame();
            _renderer.RenderScene(_sceneService.Current);
            _renderer.EndFrame();
        }

        public void Shutdown()
        {
            var logger = ServiceRegistry.Get<ILogger>();
            logger?.Info(nameof(ScenePreview), "Scene Preview shutdown");
            
            _renderer?.Shutdown();
        }

        /// <summary>
        /// Lädt eine neue Szene in die Preview
        /// </summary>
        /// <param name="scenePath">Relativer Pfad zur neuen Szene</param>
        public void LoadScene(string scenePath)
        {
            var logger = ServiceRegistry.Get<ILogger>();
            try
            {
                _sceneService?.LoadFromContent(scenePath);
                _sceneLoaded = true;
                logger?.Info(nameof(ScenePreview), $"Scene reloaded: {_sceneService?.Current?.Id}");
            }
            catch (Exception ex)
            {
                logger?.Error(nameof(ScenePreview), $"Failed to reload scene: {ex.Message}");
                _sceneLoaded = false;
            }
        }

        /// <summary>
        /// Gibt die aktuell geladene Szene zurück
        /// </summary>
        public SceneRuntime? CurrentScene => _sceneService?.Current;
    }
}
