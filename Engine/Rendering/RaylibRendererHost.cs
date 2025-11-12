using System.Numerics;
using Raylib_cs;
using System.Reflection;

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
            _cam = new Camera2D();
            _clear = clear ?? Color.Black;
        }

        public void BeginFrame()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(_clear);

            var rc = CreateRaylibCamera2D(_cam);
            Raylib.BeginMode2D((Raylib_cs.Camera2D)rc);
        }

        public void EndFrame()
        {
            Raylib.EndMode2D();
            Raylib.EndDrawing();
        }

        private static object CreateRaylibCamera2D(Camera2D cam)
        {
            var raylibAsm = typeof(Raylib_cs.Raylib).Assembly;
            var camType = raylibAsm.GetType("Raylib_cs.Camera2D") ?? typeof(Raylib_cs.Camera2D);
            var vecType = raylibAsm.GetType("Raylib_cs.Vector2");

            object camStruct = Activator.CreateInstance(camType)!; // boxed struct

            // Create Raylib Vector2 for target and offset
            object targetVec = CreateRaylibVector2(vecType, cam.Position.X, cam.Position.Y);
            object offsetVec = CreateRaylibVector2(vecType, 0f, 0f);

            // set members: try multiple name variants
            SetMemberIfExists(camStruct, camType, new[] { "target", "Target", "center", "Center" }, targetVec);
            SetMemberIfExists(camStruct, camType, new[] { "offset", "Offset" }, offsetVec);
            SetMemberIfExists(camStruct, camType, new[] { "rotation", "Rotation" }, 0f);
            SetMemberIfExists(camStruct, camType, new[] { "zoom", "Zoom" }, cam.Zoom);

            return camStruct;
        }

        private static object CreateRaylibVector2(System.Type? vecType, float x, float y)
        {
            if (vecType == null)
            {
                // fallback to System.Numerics.Vector2
                return new Vector2(x, y);
            }

            // Try to find ctor(float,float)
            var ctor = vecType.GetConstructor(new[] { typeof(float), typeof(float) });
            if (ctor != null)
                return ctor.Invoke(new object[] { x, y })!;

            // otherwise create default and set fields/properties
            object vec = Activator.CreateInstance(vecType)!;
            SetMemberIfExists(vec, vecType, new[] { "x", "X" }, x);
            SetMemberIfExists(vec, vecType, new[] { "y", "Y" }, y);
            return vec;
        }

        private static void SetMemberIfExists(object target, System.Type targetType, string[] names, object value)
        {
            foreach (var name in names)
            {
                // try field
                var f = targetType.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (f != null)
                {
                    f.SetValue(target, value);
                    return;
                }

                var p = targetType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null && p.CanWrite)
                {
                    p.SetValue(target, value);
                    return;
                }
            }
        }
    }
}
