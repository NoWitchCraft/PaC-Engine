namespace Engine.Utils
{
    public static class MathHelper
    {
        public static float Clamp(float v, float min, float max)
            => v < min ? min : (v > max ? max : v);

        public static int Clamp(int v, int min, int max)
            => v < min ? min : (v > max ? max : v);

        public static float Lerp(float a, float b, float t) => a + (b - a) * t;

        public static float InverseLerp(float a, float b, float v)
            => (b - a) == 0 ? 0f : (v - a) / (b - a);

        public static float Remap(float inMin, float inMax, float outMin, float outMax, float v)
            => Lerp(outMin, outMax, InverseLerp(inMin, inMax, v));

        public static bool AlmostEqual(float a, float b, float eps = 1e-5f)
            => System.MathF.Abs(a - b) <= eps;

        public static float WrapAngleRad(float a)
        {
            const float pi = System.MathF.PI;
            const float twoPi = 2 * System.MathF.PI;
            a %= twoPi;
            if (a <= -pi) a += twoPi;
            else if (a > pi) a -= twoPi;
            return a;
        }

        public static float WrapAngleDeg(float a)
        {
            a %= 360f;
            if (a <= -180f) a += 360f;
            else if (a > 180f) a -= 360f;
            return a;
        }
    }
}
