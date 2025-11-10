using Raylib_cs;

namespace Engine.Rendering
{
    /// <summary>
    /// Raylib-Implementierung des Frame-Hosts.
    /// </summary>
    public sealed class RaylibRendererHost : IRendererHost
    {
        private readonly Color _clear;

        public RaylibRendererHost(Color? clear = null)
        {
            _clear = clear ?? Color.Black;
        }

        public void BeginFrame()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(_clear);
            // Optional später: Raylib.BeginMode2D(camera);
        }

        public void EndFrame()
        {
            // Optional später: Raylib.EndMode2D();
            Raylib.EndDrawing();
        }
    }
}
