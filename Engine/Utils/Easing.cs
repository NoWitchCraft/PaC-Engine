namespace Engine.Utils
{
    public static class Easing
    {
        public static float Linear(float t) => t;

        public static float QuadIn(float t) => t * t;
        public static float QuadOut(float t) { t = 1 - t; return 1 - t * t; }
        public static float QuadInOut(float t) => t < 0.5f ? 2*t*t : 1 - System.MathF.Pow(-2*t + 2, 2)/2f;

        public static float CubicIn(float t) => t * t * t;
        public static float CubicOut(float t) { t = 1 - t; return 1 - t * t * t; }
        public static float CubicInOut(float t)
            => t < 0.5f ? 4*t*t*t : 1 - System.MathF.Pow(-2*t + 2, 3)/2f;

        public static float SineIn(float t)  => 1 - System.MathF.Cos((t * System.MathF.PI) / 2);
        public static float SineOut(float t) => System.MathF.Sin((t * System.MathF.PI) / 2);
        public static float SineInOut(float t) => -(System.MathF.Cos(System.MathF.PI * t) - 1) / 2f;

        public static float ExpoIn(float t)  => t <= 0 ? 0 : System.MathF.Pow(2, 10 * t - 10);
        public static float ExpoOut(float t) => t >= 1 ? 1 : 1 - System.MathF.Pow(2, -10 * t);
        public static float ExpoInOut(float t)
            => t <= 0 ? 0 : t >= 1 ? 1 :
               t < 0.5f ? System.MathF.Pow(2, 20*t - 10)/2f : (2 - System.MathF.Pow(2, -20*t + 10))/2f;

        public static float BackOut(float t, float s = 1.70158f)
        {
            t -= 1f; return t*t*((s+1)*t + s) + 1;
        }
        // Elastic/Bounce können wir später ergänzen.
    }
}
