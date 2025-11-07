# Logging & Debug System

## Overview

The PaC Engine includes a unified, configurable logging and debug system that is shared between the Engine and Editor. The system provides multiple log levels, timestamps, and flexible output options including console, file, and UI integration.

## Features

- **Multiple Log Levels**: Debug, Info, Warn, Error
- **Timestamps**: All log entries include precise timestamps (yyyy-MM-dd HH:mm:ss.fff)
- **Thread Information**: Each log entry includes the thread ID
- **Multiple Output Targets**:
  - Console output with color coding
  - File output for persistent logs
  - Event-based logging for UI integration
- **Filtering**: Filter logs by level and search text
- **Thread-Safe**: All loggers are thread-safe for concurrent use

## Usage

### Basic Logging

```csharp
using Engine.Diagnostics;

// Use the static Log class
Log.Debug("MyClass", "This is a debug message");
Log.Info("MyClass", "This is an info message");
Log.Warn("MyClass", "This is a warning");
Log.Error("MyClass", "This is an error");

// Type-safe source names
Log.Info(Log.Src<MyClass>(), "Using type-safe source");
```

### Log Levels

The system supports four log levels (in order of severity):

1. **Debug** - Detailed information for debugging purposes
2. **Info** - General informational messages
3. **Warn** - Warning messages for potentially harmful situations
4. **Error** - Error messages for serious problems

### Available Loggers

#### ConsoleLogger

Outputs log messages to the console with color coding:
- Debug: Gray
- Info: White
- Warn: Yellow
- Error: Red

```csharp
var logger = new ConsoleLogger(LogLevel.Info);
Log.Use(logger);
```

#### FileLogger

Writes log messages to a file:

```csharp
var logger = new FileLogger("path/to/logfile.log", LogLevel.Debug, append: true);
Log.Use(logger);
```

Parameters:
- `filePath`: Path to the log file
- `minLevel`: Minimum log level to write
- `append`: If true, appends to existing file; if false, overwrites

#### EventLogger

Stores log entries in memory and raises events for UI updates:

```csharp
var logger = new EventLogger(LogLevel.Debug, maxEntries: 10000);
logger.LogEntryAdded += (sender, entry) => {
    // Handle new log entry
    Console.WriteLine(entry.ToString());
};
Log.Use(logger);
```

#### MultiLogger

Dispatches log messages to multiple child loggers:

```csharp
var multiLogger = new MultiLogger(LogLevel.Debug);
multiLogger.AddLogger(new ConsoleLogger(LogLevel.Info));
multiLogger.AddLogger(new FileLogger("app.log", LogLevel.Debug));
multiLogger.AddLogger(new EventLogger(LogLevel.Debug));
Log.Use(multiLogger);
```

## Editor Integration

### Log Viewer Panel

The Editor includes a built-in Log Viewer panel accessible from the menu:

**Tools → Log Viewer...**

The Log Viewer provides:
- Real-time display of all log messages
- Color-coded log levels
- Filtering by minimum log level
- Search functionality
- Clear button to reset the log
- Scroll-through history of log entries

### Configuration

The Editor automatically configures logging on startup with:
- Console logger (Info level)
- File logger (Debug level, saved to `Logs/editor_YYYYMMDD_HHMMSS.log`)
- Event logger (Debug level, for the Log Viewer UI)

The Game project also configures multi-target logging:
- Console logger (Debug level)
- File logger (Debug level, saved to `Logs/game_YYYYMMDD_HHMMSS.log`)

## Log Entry Format

Each log entry follows this format:

```
YYYY-MM-DD HH:mm:ss.fff [TN] LEVEL Source: Message
```

Example:
```
2025-11-07 15:16:06.036 [T1] WARN  SceneService: Scene validation failed
```

Where:
- `YYYY-MM-DD HH:mm:ss.fff` - Timestamp with milliseconds
- `[TN]` - Thread ID (e.g., T1 for thread 1)
- `LEVEL` - Log level (DEBUG, INFO, WARN, ERROR)
- `Source` - Component or class name that generated the log
- `Message` - The log message

## Best Practices

1. **Use appropriate log levels**:
   - Debug: Detailed diagnostic information
   - Info: Important events and state changes
   - Warn: Unexpected situations that don't prevent operation
   - Error: Failures that require attention

2. **Provide context in messages**:
   ```csharp
   // Good
   Log.Error("SceneLoader", $"Failed to load scene '{sceneId}': {ex.Message}");
   
   // Bad
   Log.Error("SceneLoader", "Failed");
   ```

3. **Use type-safe source names**:
   ```csharp
   Log.Info(Log.Src<SceneService>(), "Scene loaded successfully");
   ```

4. **Configure appropriate log levels** for different environments:
   - Development: Debug level
   - Production: Info or Warn level

## File Locations

- **Editor logs**: `Editor/Logs/editor_YYYYMMDD_HHMMSS.log`
- **Game logs**: `Game/Logs/game_YYYYMMDD_HHMMSS.log`

Log files are automatically excluded from version control via `.gitignore`.
