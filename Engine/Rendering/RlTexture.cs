using System;
using Raylib_cs;

namespace Engine.Rendering
{
    public sealed class RlTexture : IDisposable
    {
        public Texture2D Tex { get; }
        public int Width  => Tex.width;
        public int Height => Tex.height;

        public RlTexture(Texture2D tex) => Tex = tex;

        public void Dispose()
        {
            if (Tex.id != 0) Raylib.UnloadTexture(Tex);
        }
    }
}