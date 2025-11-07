namespace Engine.Rendering
{
    /// <summary>
    /// Minimaler Frame-Host: kapselt Begin/End eines Render-Frames.
    /// (Für raylib: BeginDrawing/ClearBackground/EndDrawing)
    /// </summary>
    public interface IRendererHost
    {
        void BeginFrame();
        void EndFrame();
    }
}
