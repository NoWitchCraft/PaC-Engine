# Editor Start Hub - Feature Documentation

## Overview
The Editor Start Hub is a project management interface that appears when the PaC Engine Editor starts. It allows users to create new game projects, open existing projects, and quickly access recently opened projects.

## Features

### 1. Start Hub Window
- **Location**: `Editor/StartHub.xaml` and `Editor/StartHub.xaml.cs`
- Displayed on editor startup before the main editor window
- Dark-themed interface consistent with the editor's design
- Three main actions: New Project, Open Project, Exit
- Recent projects panel showing up to 10 recently accessed projects

### 2. New Project Creation
- **Location**: `Editor/NewProjectDialog.xaml` and `Editor/NewProjectDialog.xaml.cs`
- Allows users to:
  - Specify project name (validated for invalid characters)
  - Choose project location via folder browser
  - See full project path preview
- Automatically creates:
  - Project directory structure
  - `Content/Scenes/` subdirectories
  - `settings.json` with default values
  - `README.md` file

### 3. Open Existing Project
- File dialog for selecting `settings.json` from existing projects
- Validation that the file exists and is readable
- Option to auto-create missing `settings.json` if project folder is valid

### 4. Recent Projects
- **Location**: `Editor/RecentProjectsManager.cs`
- Automatically tracks up to 10 most recently opened projects
- Persisted to: `%AppData%/PaCEngine/RecentProjects.json`
- Displays: Project name, full path, last opened timestamp
- Validates project paths (removes entries for deleted projects)
- Double-click to open a recent project

### 5. Project Information Model
- **Location**: `Editor/ProjectInfo.cs`
- Stores: Name, Path, LastOpened timestamp
- JSON-serializable for persistence

### 6. Project Helper Utilities
- **Location**: `Editor/ProjectHelper.cs`
- Shared utility for creating default `settings.json`
- Ensures consistency across New Project and Open Project features

## Integration with Editor

### App Startup Flow
1. Editor starts → `App.xaml.cs` → `Application_Startup` event
2. Show `StartHub` window as modal dialog
3. User selects/creates a project
4. Initialize engine with selected project path:
   - Load `settings.json` from project directory
   - Configure `ContentResolver` with project's content root
   - Register services in `ServiceRegistry`
5. Open `MainWindow` with initialized engine
6. If user cancels/exits StartHub → shutdown application

### MainWindow Integration
- Updated to use `ContentResolver` for scene file dialogs
- Scene open/save dialogs default to `{ProjectPath}/Content/Scenes/`
- No more hardcoded paths to `Game/Content/Scenes`

## File Structure Created by New Project

```
MyGame/
├── Content/
│   └── Scenes/
├── settings.json
└── README.md
```

## Settings File Format

The default `settings.json` created for new projects:

```json
{
  "ContentRoot": "Content",
  "StartSceneId": "",
  "WindowWidth": 1280,
  "WindowHeight": 720,
  "Language": "de-DE",
  "MasterVolume": 1.0
}
```

## Error Handling

### Invalid Paths
- Validates project names for invalid filesystem characters
- Checks if project directories exist before opening
- Provides user-friendly error messages

### Missing settings.json
- Offers to create default settings.json when opening a project
- Validates settings file can be loaded before proceeding

### Recent Projects
- Silently handles corrupt or inaccessible recent projects file
- Filters out deleted projects from recent list
- Gracefully handles I/O and JSON errors

## Dependencies

### New NuGet Package
- **System.Windows.Forms** (v8.0.0)
  - Used for `FolderBrowserDialog` in New Project dialog
  - No known vulnerabilities

## User Interface

### Start Hub Layout
- **Left Panel**: Quick Actions (New, Open, Exit buttons)
- **Right Panel**: Recent Projects list
- Dark theme (#1e1e1e background)
- Responsive hover states on buttons and list items

### New Project Dialog
- Project name input field
- Location selector with browse button
- Live preview of full project path
- Validation feedback (color-coded)
- Create/Cancel buttons

## Future Enhancements

Potential improvements for future versions:
- Project templates (different game types)
- Project settings editor in Start Hub
- Import/export project configuration
- Cloud storage integration for recent projects
- Project search/filter functionality
- Thumbnail previews of projects

## Technical Notes

### Thread Safety
- Recent projects list is accessed on UI thread only
- File I/O operations wrapped in try-catch blocks
- Specific exception types handled (JsonException, IOException, UnauthorizedAccessException)

### Performance
- Recent projects loaded on-demand when Start Hub shows
- Invalid projects filtered during load
- Minimal overhead on startup

### Cross-Platform Considerations
- Currently Windows-only (WPF application)
- File paths use `Path.Combine` for compatibility
- AppData location uses `Environment.SpecialFolder.ApplicationData`
