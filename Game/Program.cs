using Engine.Core;
using Engine.Diagnostics;
using Engine.Rendering;
using Engine.Resources;
using Raylib_cs;

namespace Game
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // App & Services hochfahren
            var app = new GameApp();
            app.Initialize();

            // Fenster öffnen
            Raylib.InitWindow(1280, 720, "PaCEngine (raylib)");
            Raylib.SetTargetFPS(60);

            // Render-Infrastruktur
            var renderer = new Renderer();                 // deine Layer-Pipeline
            var updates  = new UpdateHub();                // tickt IUpdatable (Anim etc.)
            var host     = new RaylibRendererHost(Color.Black);
            var input    = ServiceRegistry.Get<Engine.Input.IInputService>();

            // Hier (optional) Renderables registrieren (BG-Sprite, Hotspot-Gizmos)
            //   var scenes = ServiceRegistry.Get<ISceneService>();
            //   var rm = ServiceRegistry.Get<IResourceManager>();
            //   // bg = rm.Get<RlTexture>(scenes.Current!.BackgroundPath) -> Sprite -> renderer.Add(bg)
            //   // foreach hotspot -> gizmo -> renderer.Add(gizmo)

            // Loop starten (fixed 60 Hz Updates, Render pro Frame)
            var loop = new GameLoop(app, input, updates, renderer, host, fixedDt: 1f / 60f, maxStepsPerFrame: 5);
            loop.Run(
                shouldClose: () => Raylib.WindowShouldClose(),
                getFrameTimeSeconds: () => Raylib.GetFrameTime()
            );

            // Shutdown & Cleanup
            app.Shutdown();
            ServiceRegistry.Get<IResourceManager>().UnloadAll();
            Raylib.CloseWindow();
        }
    }
}
