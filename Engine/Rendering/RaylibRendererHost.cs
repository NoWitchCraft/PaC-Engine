using System.Numerics;
using Raylib_cs;

namespace Engine.Rendering
{
    /// <summary>
    /// Raylib-Implementierung des Frame-Hosts.
    /// </summary>
    public sealed class RaylibRendererHost : IRendererHost
    {
        private readonly Camera2D _cam;
        private readonly Color _clear;

        public RaylibRendererHost(Color? clear = null)
        {
            _cam = camera;
            _clear = clear ?? Color.Black;
        }

        public void BeginFrame()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(_clear);
            
            var rc = new Raylib_cs.Camera2D
            {
                target = new Vector2(_cam.Position.X, _cam.Position.Y),
                offset = Vector2.Zero,
                rotation = 0f,
                zoom = _camera.Zoom
            };
            Raylib.BeginMode2D(rc);
        }

        public void EndFrame()
        {
            Raylib.EndMode2D();
            Raylib.EndDrawing();
        }
    }
}
