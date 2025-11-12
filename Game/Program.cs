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
            var cam      = new Engine.Rendering.Camera2D(1280, 720);
            var host     = new RaylibRendererHost(Color.Black);

            // Kamera global verfügbar machen (für HitTest etc.)
            ServiceRegistry.Register<Camera2D>(cam);

            // Services holen
            var rm     = ServiceRegistry.Get<IResourceManager>();
            var scenes = ServiceRegistry.Get<ISceneService>();

            // Hintergrund-Textur LADEN (nach InitWindow!) + Sprite registrieren
            var bgTex = rm.Get<Engine.Rendering.RlTexture>(scenes.Current!.BackgroundPath);
            var bg    = new Engine.Rendering.Sprite(bgTex)
            {
                Layer = Engine.Rendering.RenderLayer.Background,
                Z = 0,
                Position = new Engine.Common.Vector2(0, 0)
            };
            renderer.Add(bg);

            // Kamera-Bounds = Größe der Szene (hier: BG)
            cam.SetBounds(new Engine.Common.RectF(0, 0, bgTex.Width, bgTex.Height));
            cam.SetViewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

            var input    = ServiceRegistry.Get<Engine.Input.IInputService>();

            // Loop starten (fixed 60 Hz Updates, Render pro Frame)
            var loop = new GameLoop(app, input, updates, renderer, host, fixedDt: 1f / 60f, maxStepsPerFrame: 5);
            loop.Run(
                shouldClose: () => Raylib.WindowShouldClose(),
                getFrameTimeSeconds: () =>
                {
                    // Viewport bei Resize nachziehen
                    cam.SetViewport(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
                    return Raylib.GetFrameTime();
                }
            );

            // Shutdown & Cleanup
            app.Shutdown();
            ServiceRegistry.Get<IResourceManager>().UnloadAll();
            Raylib.CloseWindow();
        }
    }
}
