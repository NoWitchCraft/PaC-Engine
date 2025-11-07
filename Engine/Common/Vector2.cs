namespace Engine.Common
{
    /// <summary>
    /// Ein einfacher 2D-Vektor (X,Y) mit Grundrechenarten und Hilfsfunktionen.
    /// </summary>
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Zero() => new Vector2(0, 0);

        // --- Utility-Funktionen ---
        public float Length => System.MathF.Sqrt(X * X + Y * Y);
        public float LengthSquared => X * X + Y * Y;

        public Vector2 Normalized()
        {
            var len = Length;
            if (len <= 1e-6f) return new Vector2(0, 0);
            return new Vector2(X / len, Y / len);
        }

        public static float Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;

        public static float Distance(Vector2 a, Vector2 b)
            => System.MathF.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
            => new Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);

        // --- Operatoren ---
        public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, float f) => new(a.X * f, a.Y * f);
        public static Vector2 operator /(Vector2 a, float f) => new(a.X / f, a.Y / f);

        public override string ToString() => $"({X:0.###}, {Y:0.###})";
    }
}
