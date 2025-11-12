using Engine.Diagnostics;
using Engine.Rendering;
using Engine.Runtime;
using Raylib_cs;

namespace Game
{
    public static class HitTestHelper
    {
        public static void LogHitUnderMouse()
        {
            var cam    = ServiceRegistry.Get<Camera2D>();
            var scenes = ServiceRegistry.Get<ISceneService>();

            var mp    = Raylib.GetMousePosition();
            var world = cam.ScreenToWorld(mp.X, mp.Y);

            var hit = scenes.HitTest(world.X, world.Y);
            if (hit != null)
                Log.Info("HitTest", $"Hit {hit.Id} at ({world.X:0.##},{world.Y:0.##})");
            else
                Log.Info("HitTest", $"No hit at ({world.X:0.##},{world.Y:0.##})");
        }
    }
}
