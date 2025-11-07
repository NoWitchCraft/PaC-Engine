using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Diagnostics
{
    /// <summary>
    /// Logger that dispatches log entries to multiple child loggers.
    /// </summary>
    public sealed class MultiLogger : ILogger
    {
        private readonly List<ILogger> _loggers = new List<ILogger>();
        private LogLevel _minimumLevel;

        public LogLevel MinimumLevel
        {
            get => _minimumLevel;
            set
            {
                _minimumLevel = value;
                // Update all child loggers
                foreach (var logger in _loggers)
                    logger.MinimumLevel = value;
            }
        }

        public MultiLogger(LogLevel minLevel = LogLevel.Info)
        {
            _minimumLevel = minLevel;
        }

        public void AddLogger(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _loggers.Add(logger);
            logger.MinimumLevel = _minimumLevel;
        }

        public void RemoveLogger(ILogger logger)
        {
            _loggers.Remove(logger);
        }

        public void Log(LogLevel level, string source, string message)
        {
            if (level < MinimumLevel) return;

            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(level, source, message);
                }
                catch
                {
                    // Suppress errors from individual loggers to avoid disrupting application
                }
            }
        }
    }
}
