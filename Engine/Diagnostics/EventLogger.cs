using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Diagnostics
{
    /// <summary>
    /// Logger that stores log entries in memory and raises events for UI updates.
    /// Thread-safe for concurrent logging.
    /// </summary>
    public sealed class EventLogger : ILogger
    {
        private readonly ConcurrentQueue<LogEntry> _entries = new ConcurrentQueue<LogEntry>();
        private readonly int _maxEntries;

        public LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Event raised when a new log entry is added.
        /// </summary>
        public event EventHandler<LogEntry>? LogEntryAdded;

        public EventLogger(LogLevel minLevel = LogLevel.Info, int maxEntries = 10000)
        {
            MinimumLevel = minLevel;
            _maxEntries = maxEntries;
        }

        public void Log(LogLevel level, string source, string message)
        {
            if (level < MinimumLevel) return;

            var entry = new LogEntry(level, source, message, Thread.CurrentThread.ManagedThreadId);
            _entries.Enqueue(entry);

            // Maintain max entries limit
            while (_entries.Count > _maxEntries)
            {
                _entries.TryDequeue(out _);
            }

            // Raise event on a separate thread to avoid blocking the logger
            LogEntryAdded?.Invoke(this, entry);
        }

        /// <summary>
        /// Gets all log entries currently stored.
        /// </summary>
        public IReadOnlyList<LogEntry> GetEntries()
        {
            return _entries.ToArray();
        }

        /// <summary>
        /// Clears all stored log entries.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
        }
    }
}
