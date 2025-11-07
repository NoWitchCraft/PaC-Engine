using System.Diagnostics;

namespace Engine.Utils
{
    public sealed class Time
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private long _lastTicks = 0;

        public float DeltaTime { get; private set; }
        public float TotalTime => (float)_sw.Elapsed.TotalSeconds;

        public void Tick()
        {
            long t = _sw.ElapsedTicks;
            if (_lastTicks == 0) { _lastTicks = t; DeltaTime = 0f; return; }
            long dtTicks = t - _lastTicks;
            _lastTicks = t;
            DeltaTime = (float)dtTicks / Stopwatch.Frequency;
        }
    }
}
