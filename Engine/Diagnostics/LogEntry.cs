using System;

namespace Engine.Diagnostics
{
    /// <summary>
    /// Represents a single log entry with timestamp and metadata.
    /// </summary>
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; init; }
        public LogLevel Level { get; init; }
        public string Source { get; init; }
        public string Message { get; init; }
        public int ThreadId { get; init; }

        public LogEntry(LogLevel level, string source, string message, int threadId)
        {
            Timestamp = DateTime.UtcNow;
            Level = level;
            Source = source ?? "-";
            Message = message ?? "";
            ThreadId = threadId;
        }

        public override string ToString()
        {
            var ts = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            var lvl = Level.ToString().ToUpperInvariant().PadRight(5);
            return $"{ts} [T{ThreadId}] {lvl} {Source}: {Message}";
        }
    }
}
