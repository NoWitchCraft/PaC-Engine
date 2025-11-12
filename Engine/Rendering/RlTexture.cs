using System;
using Raylib_cs;
using System.Reflection;

namespace Engine.Rendering
{
    public sealed class RlTexture : IDisposable
    {
        public Texture2D Tex { get; }
        public int Width { get; }
        public int Height { get; }
        public int Id { get; }

        public RlTexture(Texture2D tex)
        {
            Tex = tex;
            Width = GetIntMember(tex, "width") ?? GetIntMember(tex, "Width") ?? 0;
            Height = GetIntMember(tex, "height") ?? GetIntMember(tex, "Height") ?? 0;
            Id = GetIntMember(tex, "id") ?? GetIntMember(tex, "Id") ?? 0;
        }

        private static int? GetIntMember(object obj, string name)
        {
            if (obj == null) return null;
            var t = obj.GetType();
            var f = t.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
            {
                var v = f.GetValue(obj);
                if (v != null) return Convert.ToInt32(v);
            }
            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (p != null)
            {
                var v = p.GetValue(obj);
                if (v != null) return Convert.ToInt32(v);
            }
            return null;
        }

        public void Dispose()
        {
            try
            {
                // If Id is zero, likely empty texture; Raylib.UnloadTexture should handle it, but keep check.
                if (Id != 0)
                    Raylib.UnloadTexture(Tex);
            }
            catch
            {
                // swallow errors during dispose
            }
        }
    }
}