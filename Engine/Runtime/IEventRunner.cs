namespace Engine.Runtime
{
    public interface IEventRunner
    {
        void Trigger(string eventPathId);
    }

    // Minimal-Stub (nur Log); später echte Sequencer/Graph
    public sealed class LogEventRunner : IEventRunner
    {
        public void Trigger(string eventPathId)
        {
            Engine.Diagnostics.Log.Info(nameof(LogEventRunner), $"Trigger event: {eventPathId}");
        }
    }
}
