using System;
using Engine.Core;
using Engine.Diagnostics;
using Engine.Input;
using Engine.Rendering;

namespace Engine.Core
{
    /// <summary>
    /// Fixed-timestep GameLoop:
    /// Ablauf pro Frame: Input → (0..n) FixedUpdate → Render.
    /// </summary>
    public sealed class GameLoop
    {
        private readonly IGameApp _app;
        private readonly IInputService _input;
        private readonly UpdateHub _updates;
        private readonly Renderer _renderer;
        private readonly IRendererHost _host;

        private readonly float _fixedDt;
        private readonly int _maxStepsPerFrame;

        public GameLoop(
            IGameApp app,
            IInputService input,
            UpdateHub updates,
            Renderer renderer,
            IRendererHost host,
            float fixedDt = 1f / 60f,
            int maxStepsPerFrame = 5)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _updates = updates ?? throw new ArgumentNullException(nameof(updates));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _host = host ?? throw new ArgumentNullException(nameof(host));

            _fixedDt = fixedDt;
            _maxStepsPerFrame = Math.Max(1, maxStepsPerFrame);
        }

        /// <summary>
        /// Führt den Loop aus, bis der Host ein Close signalisiert.
        /// Der Host entscheidet, wann abgebrochen wird (z. B. WindowShouldClose).
        /// </summary>
        public void Run(Func<bool> shouldClose, Func<float> getFrameTimeSeconds)
        {
            float accumulator = 0f;

            while (!shouldClose())
            {
                // Variable Delta vom Host (raylib: GetFrameTime)
                float frameDt = MathF.Max(0f, getFrameTimeSeconds());
                accumulator += frameDt;

                // 1) Input (pro Frame — Events/Kanten werden erkannt)
                _input.Update(frameDt);

                // 2) Fixed Updates nachholen (Clamp gegen Spiral of Death)
                int steps = 0;
                while (accumulator >= _fixedDt && steps < _maxStepsPerFrame)
                {
                    _app.Update(_fixedDt);
                    _updates.Update(_fixedDt);
                    accumulator -= _fixedDt;
                    steps++;
                }
                if (steps == _maxStepsPerFrame && accumulator >= _fixedDt)
                {
                    // Wir droppen überschüssige Zeit – oder alternativ clampen:
                    accumulator = 0f;
                    Log.Warn(nameof(GameLoop), "Update overload: dropping accumulated time to avoid spiral.");
                }

                // 3) Render
                _host.BeginFrame();
                _renderer.DrawAll();
                _host.EndFrame();
            }
        }
    }
}
