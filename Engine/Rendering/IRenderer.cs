using Engine.Runtime;

namespace Engine.Rendering
{
    /// <summary>
    /// Interface für Rendering-Backend
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Initialisiert das Rendering-System
        /// </summary>
        void Initialize();

        /// <summary>
        /// Beginnt einen neuen Render-Frame
        /// </summary>
        void BeginFrame();

        /// <summary>
        /// Rendert die aktuelle Szene
        /// </summary>
        /// <param name="scene">Die zu rendernde Szene</param>
        void RenderScene(SceneRuntime? scene);

        /// <summary>
        /// Beendet den aktuellen Render-Frame
        /// </summary>
        void EndFrame();

        /// <summary>
        /// Räumt Rendering-Ressourcen auf
        /// </summary>
        void Shutdown();
    }
}
