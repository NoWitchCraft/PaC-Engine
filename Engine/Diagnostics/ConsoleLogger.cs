using System;
using System.Globalization;
using System.Threading;

namespace Engine.Diagnostics
{
    public sealed class ConsoleLogger : ILogger
    {
        public LogLevel MinimumLevel { get; set; }

        public ConsoleLogger(LogLevel minLevel = LogLevel.Info)
        {
            MinimumLevel = minLevel;
        }

        public void Log(LogLevel level, string source, string message)
        {
            if(level < MinimumLevel) return;

            var ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var tid = Thread.CurrentThread.ManagedThreadId;
            var lvl = level.ToString().ToUpperInvariant().PadRight(5);
            var src = string.IsNullOrWhiteSpace(source) ? "-" : source;

            var line =$"{ts} [T{tid}] {lvl} {src}: {message}";

            // einfache Farbgebung
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.White,
                LogLevel.Warn => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            Console.WriteLine(line);
            Console.ForegroundColor = prev;
        }
    }
}