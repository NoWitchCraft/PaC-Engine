using System;
using System.Diagnostics;
using System.Threading;

namespace Engine
{
    /// <summary>
    /// The smallest interface for a game that the engine hosts
    /// </summary>
    public interface IGameApp
    {
        void Initialize();
        void Update(float dt);
        void Render();
        void Shutdown();
    }

    /// <summary>
    /// Führt ein IGameApp mit einem simplen Fixed-Rate-Loop aus (~60 FPS).
    /// Update und Render sind getrennt für bessere Wartbarkeit.
    /// </summary>
    public sealed class EngineHost
    {
        private readonly IGameApp _app;
        private readonly int _targetFps;
        private readonly double _targetFrameTimeMs;

        public EngineHost(IGameApp app, int targetFps = 60)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _targetFps = Math.Max(1, targetFps);
            _targetFrameTimeMs = 1000.0 / _targetFps;
        }

        public void Run(Func<bool>? shouldExit = null)
        {
            _app.Initialize();
            var sw = new Stopwatch();
            sw.Start();

            long prevTicks = sw.ElapsedTicks;
            double tickFreq = 1000.0 / Stopwatch.Frequency; //ms per tick

            bool running = true;
            while (running)
            {
                // Exit-Bedingung von außen (z.B. Tastendruck)
                if (shouldExit != null && shouldExit()) break;

                long nowTicks = sw.ElapsedTicks;
                double frameMs = (nowTicks - prevTicks) * tickFreq;
                prevTicks = nowTicks;

                float dt = (float)(frameMs / 1000.0);
                
                // Update-Phase: Logik, Physik, Input
                _app.Update(dt);
                
                // Render-Phase: Zeichnen der Szene
                _app.Render();

                // Throttle für ~TargetFPS
                double sleepMs = _targetFrameTimeMs - frameMs;
                if (sleepMs > 0) Thread.Sleep((int)Math.Floor(sleepMs));
            }

            _app.Shutdown();
        }
    }
}