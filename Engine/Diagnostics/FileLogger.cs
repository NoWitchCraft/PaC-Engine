using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Engine.Diagnostics
{
    /// <summary>
    /// Logger that writes log entries to a file.
    /// </summary>
    public sealed class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private readonly bool _append;

        public LogLevel MinimumLevel { get; set; }

        public FileLogger(string filePath, LogLevel minLevel = LogLevel.Info, bool append = true)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            MinimumLevel = minLevel;
            _append = append;

            // Ensure directory exists
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Create or clear file
            if (!_append && File.Exists(_filePath))
                File.WriteAllText(_filePath, string.Empty);
        }

        public void Log(LogLevel level, string source, string message)
        {
            if (level < MinimumLevel) return;

            var entry = new LogEntry(level, source, message, Thread.CurrentThread.ManagedThreadId);
            var line = entry.ToString();

            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_filePath, line + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // Suppress file write errors to avoid disrupting application
                }
            }
        }
    }
}
