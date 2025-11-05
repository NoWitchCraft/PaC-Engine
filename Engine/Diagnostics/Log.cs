using System;

namespace Engine.Diagnostics
{
    public static class Log
    {
        private static ILogger _logger = new ConsoleLogger(LogLevel.Info);

        public static void Use(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static LogLevel MinimumLevel
        {
            get => _logger.MinimumLevel;
            set => _logger.MinimumLevel = value;
        }

        public static void Write(LogLevel level, string source, string message)
            => _logger.Log(level, source, message);

        // Kurzformen
        public static void Debug(string source, string message) => _logger.Debug(source, message);
        public static void Info (string source, string message) => _logger.Info (source, message);
        public static void Warn (string source, string message) => _logger.Warn (source, message);
        public static void Error(string source, string message) => _logger.Error(source, message);

        // Typsichere Quelle
        public static string Src<T>() => typeof(T).Name;
    }
}
