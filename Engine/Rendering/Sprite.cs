using Engine.Common;
using Raylib_cs;

namespace Engine.Rendering
{
    public sealed class Sprite : IRenderable
    {
        public RenderLayer Layer { get; set; } = RenderLayer.Background;
        public int Z { get; set; } = 0;

        public RlTexture Texture { get; }
        public Vector2 Position { get; set; } = new Vector2(0, 0);

        int IRenderable.Layer => throw new NotImplementedException();

        public Sprite(RlTexture tex) => Texture = tex;

        public void Draw()
        {
            Raylib.DrawTexture(Texture.Tex, (int)Position.X, (int)Position.Y, Color.White);
        }
    }
}