using System;

namespace Engine.Diagnostics
{
    public interface ILogger
    {
        LogLevel MinimumLevel { get; set; }

        void Log(LogLevel level, string source, string message);

        // Komfort-Hilfen
        void Debug(string source, string message) => Log(LogLevel.Debug, source, message);
        void Info(string source, string message) => Log(LogLevel.Info, source, message);
        void Warn(string source, string message) => Log(LogLevel.Warn, source, message);
        void Error(string source, string message) => Log(LogLevel.Error, source, message);
        
    }
}