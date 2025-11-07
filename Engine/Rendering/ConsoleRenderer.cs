using System;
using System.Text;
using Engine.Diagnostics;
using Engine.Runtime;

namespace Engine.Rendering
{
    /// <summary>
    /// Einfacher Console-basierter Renderer für Preview-Zwecke.
    /// Zeigt Szeneninformationen im Console-Output an.
    /// </summary>
    public sealed class ConsoleRenderer : IRenderer
    {
        private readonly ILogger? _log;
        private int _frameCount;
        private bool _initialized;

        public ConsoleRenderer(ILogger? log = null)
        {
            _log = log;
        }

        public void Initialize()
        {
            _initialized = true;
            _frameCount = 0;
            _log?.Info(nameof(ConsoleRenderer), "Console Renderer initialized");
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║   PaC Engine - Scene Preview          ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine();
        }

        public void BeginFrame()
        {
            if (!_initialized) return;
            _frameCount++;
        }

        public void RenderScene(SceneRuntime? scene)
        {
            if (!_initialized || scene == null) return;

            // Nur alle 60 Frames rendern (1x pro Sekunde bei 60 FPS)
            if (_frameCount % 60 != 0) return;

            var sb = new StringBuilder();
            sb.AppendLine($"\n[Frame {_frameCount}] Rendering Scene: {scene.Id}");
            sb.AppendLine($"  Background: {scene.BackgroundPath}");
            sb.AppendLine($"  Hotspots: {scene.Hotspots.Count}");
            
            if (scene.Hotspots.Count > 0)
            {
                sb.AppendLine("  ┌─ Hotspots ──────────────────────────");
                foreach (var hotspot in scene.Hotspots)
                {
                    sb.AppendLine($"  │ • {hotspot.Id}");
                    sb.AppendLine($"  │   Position: ({hotspot.Rect.X:F0}, {hotspot.Rect.Y:F0})");
                    sb.AppendLine($"  │   Size: {hotspot.Rect.Width:F0}x{hotspot.Rect.Height:F0}");
                    if (!string.IsNullOrWhiteSpace(hotspot.EventPathId))
                        sb.AppendLine($"  │   Event: {hotspot.EventPathId}");
                }
                sb.AppendLine("  └─────────────────────────────────────");
            }

            if (!string.IsNullOrWhiteSpace(scene.MusicCueId))
            {
                sb.AppendLine($"  Music: {scene.MusicCueId}");
            }

            Console.WriteLine(sb.ToString());
        }

        public void EndFrame()
        {
            // Nichts zu tun für Console-Renderer
        }

        public void Shutdown()
        {
            if (!_initialized) return;
            
            _log?.Info(nameof(ConsoleRenderer), $"Console Renderer shutdown. Total frames: {_frameCount}");
            Console.WriteLine();
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine("  Rendering stopped.");
            Console.WriteLine($"  Total frames rendered: {_frameCount}");
            Console.WriteLine("════════════════════════════════════════");
            _initialized = false;
        }
    }
}
