# Log Viewer Panel - Visual Guide

## Menu Access
```
PaCEngine Editor
┌─────────────────────────────────────────────────────┐
│ File   Scene   Tools                                │
│                 └── Validate Scene                  │
│                 └── ───────────────                 │
│                 └── Log Viewer... ← NEW!            │
└─────────────────────────────────────────────────────┘
```

## Log Viewer Window
```
┌────────────────────────────────── Log Viewer ──────────────────────────────────┐
│                                                                                 │
│  Min Level: [Debug ▼]    Search: [____________]    [Clear]  [Close]           │
│  ───────────────────────────────────────────────────────────────────────────   │
│                                                                                 │
│  2025-11-07 15:16:05.984  DEBUG  TestApp          This is a debug message      │
│  2025-11-07 15:16:06.035  INFO   TestApp          This is an info message      │
│  2025-11-07 15:16:06.036  WARN   SceneValidator   Scene has 2 warnings         │
│  2025-11-07 15:16:06.036  ERROR  SceneLoader      Failed to load scene         │
│  2025-11-07 15:16:06.100  INFO   MainWindow        Scene loaded: test_scene    │
│  2025-11-07 15:16:07.200  INFO   MainWindow        Added hotspot: door         │
│  2025-11-07 15:16:08.500  WARN   ResourceManager   Texture not found: bg.png   │
│  2025-11-07 15:16:09.100  INFO   MainWindow        Saved scene to file         │
│                                                                                 │
│  ───────────────────────────────────────────────────────────────────────────   │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘

Color Coding:
  DEBUG - Gray      (#808080)
  INFO  - White     (#FFFFFF)
  WARN  - Yellow    (#FFD700)
  ERROR - Red       (#FF6464)
```

## Features

### 1. Real-time Updates
Log entries appear instantly as they're generated throughout the Engine and Editor.

### 2. Level Filtering
Use the "Min Level" dropdown to filter:
- **Debug** - Shows all logs (Debug, Info, Warn, Error)
- **Info** - Shows Info, Warn, Error (hides Debug)
- **Warn** - Shows Warn, Error (hides Debug, Info)
- **Error** - Shows only Error logs

### 3. Text Search
Type in the search box to filter logs by:
- Source component name
- Message content
- Case-insensitive matching

### 4. Clear Function
Click "Clear" to remove all log entries from the viewer (does not affect log files).

## Usage Examples

### Scenario 1: Finding Errors
1. Open Log Viewer
2. Set Min Level to "Error"
3. Only error messages are displayed
4. Click on any error to see full details

### Scenario 2: Debugging a Feature
1. Open Log Viewer
2. Set Min Level to "Debug"
3. Type feature name in Search (e.g., "SceneService")
4. See only logs related to that component

### Scenario 3: Monitoring Real-time Activity
1. Keep Log Viewer open in separate window
2. Work in Editor (load scenes, add hotspots, etc.)
3. Watch logs appear in real-time
4. Color-coded warnings/errors catch your attention immediately

## Log File Location

In addition to the Log Viewer, all logs are automatically saved to disk:

**Editor Logs:**
```
PaCEngine/Editor/Logs/editor_20251107_151605.log
```

**Game Logs:**
```
PaCEngine/Game/Logs/game_20251107_160234.log
```

Log files use the same format as the viewer but without color codes.

## Integration with Development Workflow

### During Development
- Keep Log Viewer open to monitor Editor operations
- Debug level shows detailed diagnostic information
- Search for specific components when investigating issues

### During Testing
- Set level to Warn or Error to focus on issues
- Review log files after test runs
- Use search to find specific test scenarios

### For Users (when distributed)
- Log files capture all activity for troubleshooting
- Users can send log files when reporting issues
- UTC timestamps ensure consistency across time zones

## Technical Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Application Code                     │
│                  (Engine + Editor)                       │
└────────────────────┬────────────────────────────────────┘
                     │ Log.Info(), Log.Error(), etc.
                     ↓
┌─────────────────────────────────────────────────────────┐
│                    MultiLogger                          │
│              (Dispatches to all loggers)                │
└─────┬────────────────────┬─────────────────┬───────────┘
      │                    │                 │
      ↓                    ↓                 ↓
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│ConsoleLogger │  │ FileLogger   │  │EventLogger   │
│              │  │              │  │              │
│ • Color      │  │ • Disk Save  │  │ • UI Events  │
│ • Real-time  │  │ • Persistent │  │ • In-Memory  │
└──────────────┘  └──────────────┘  └──────┬───────┘
                                           │
                                           ↓
                                   ┌──────────────┐
                                   │ Log Viewer   │
                                   │   (WPF UI)   │
                                   │              │
                                   │ • Filtering  │
                                   │ • Search     │
                                   │ • Colors     │
                                   └──────────────┘
```

## Benefits

✅ **Unified System** - Same logging API for Engine and Editor
✅ **Multiple Outputs** - Console, file, and UI simultaneously
✅ **Filterable** - Find exactly what you need quickly
✅ **Color-Coded** - Errors stand out visually
✅ **Non-Intrusive** - Logging failures won't crash the app
✅ **Thread-Safe** - Works correctly in multi-threaded scenarios
✅ **Configurable** - Set different levels per logger
✅ **Professional** - Production-ready logging infrastructure
