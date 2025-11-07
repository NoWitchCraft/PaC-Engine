namespace Engine.Common
{
    /// <summary>2D-Rechteck in float-Koordinaten.</summary>
    public struct RectF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width  { get; set; }
        public float Height { get; set; }

        public RectF(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        // Komfort
        public float Left   => X;
        public float Top    => Y;
        public float Right  => X + Width;
        public float Bottom => Y + Height;
        public bool  IsEmpty => Width <= 0f || Height <= 0f;

        // Tests/Operationen
        public bool Contains(float x, float y)
            => x >= Left && x <= Right && y >= Top && y <= Bottom;

        public bool Contains(Vector2 p) => Contains(p.X, p.Y);

        public bool Intersects(in RectF other)
            => !(other.Left > Right || other.Right < Left || other.Top > Bottom || other.Bottom < Top);

        public RectF Inflate(float dx, float dy)
            => new RectF(X - dx, Y - dy, Width + 2 * dx, Height + 2 * dy);

        public RectF Offset(float dx, float dy)
            => new RectF(X + dx, Y + dy, Width, Height);

        public Vector2 Center => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

        public static RectF Union(in RectF a, in RectF b)
        {
            float x1 = System.MathF.Min(a.Left,  b.Left);
            float y1 = System.MathF.Min(a.Top,   b.Top);
            float x2 = System.MathF.Max(a.Right, b.Right);
            float y2 = System.MathF.Max(a.Bottom,b.Bottom);
            return new RectF(x1, y1, x2 - x1, y2 - y1);
        }

        public static RectF Intersect(in RectF a, in RectF b)
        {
            float x1 = System.MathF.Max(a.Left,  b.Left);
            float y1 = System.MathF.Max(a.Top,   b.Top);
            float x2 = System.MathF.Min(a.Right, b.Right);
            float y2 = System.MathF.Min(a.Bottom,b.Bottom);
            if (x2 <= x1 || y2 <= y1) return new RectF(0, 0, 0, 0);
            return new RectF(x1, y1, x2 - x1, y2 - y1);
        }

        public override string ToString() => $"[{X:0.###},{Y:0.###},{Width:0.###},{Height:0.###}]";
    }
}
