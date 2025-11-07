# Scene Validation & Status Feedback

## Overview
The PaC Engine Editor now includes comprehensive scene validation that automatically checks scenes for inconsistencies and provides real-time feedback to users.

## Features

### Automatic Validation
- **On Load**: Scenes are automatically validated when opened
- **On Save**: Validation runs before saving, with option to save despite errors
- **On Property Change**: Auto-validation triggers when scene or hotspot IDs are modified
- **Manual Validation**: Press F5 or use Tools → Validate Scene

### Status Feedback

#### Status Bar Indicators
- **Main Status**: Shows validation results with color-coded text
  - 🟢 Green: No issues (OK)
  - 🟡 Yellow: Warnings only
  - 🔴 Red: Errors detected
- **Validation Status**: Dedicated indicator showing error/warning count

#### Validation Panel
- Collapsible panel at bottom of editor
- Lists all validation issues with severity icons:
  - ❌ Error
  - ⚠️ Warning
  - ℹ️ Info
- Double-click any issue to navigate to the affected item

#### Visual Indicators
- Items with validation issues are marked with ⚠️ icon in the hierarchy

### Validation Rules

#### Scene-Level Validation
- **SCENE_ID_EMPTY**: Scene ID must not be empty
- **BG_MISSING**: Background path must be specified

#### Hotspot Validation
- **HS_ID_EMPTY**: Hotspot ID must not be empty
- **HS_ID_DUP**: Hotspot IDs must be unique
- **HS_RECT_NULL**: Hotspot must have a valid rectangle
- **HS_W_LEQ_0**: Rectangle width must be greater than 0
- **HS_H_LEQ_0**: Rectangle height must be greater than 0
- **HS_POS_NEG**: Warning for negative X/Y positions
- **HS_LABEL_EMPTY**: Warning for empty label keys (optional)

### Auto-Fix Functionality

The editor can automatically fix common validation issues:

#### Context Menu Fix
1. Right-click on a hotspot with issues in the hierarchy
2. Select "Fix Validation Issues"
3. The following issues are auto-fixed:
   - Empty IDs: Generated unique IDs
   - Empty LabelKeys: Set to match hotspot ID
   - Null rectangles: Create default rectangle (64, 64, 160x80)
   - Invalid dimensions: Reset to defaults (160x80)
   - Negative positions: Reset to (0, 0)
   - Duplicate IDs: Generate unique IDs with suffix

### Keyboard Shortcuts
- **F5**: Validate current scene
- **Ctrl+S**: Save scene (includes validation check)

### Best Practices

1. **Regular Validation**: Press F5 periodically while editing
2. **Review Warnings**: While warnings don't prevent saving, they may indicate design issues
3. **Use Auto-Fix**: For bulk issues, use the auto-fix feature to quickly resolve common problems
4. **Check Before Save**: Always review validation errors before saving with errors

### Testing Validation

A test scene with various validation issues is included at:
`Game/Content/Scenes/test_validation.scene.json`

This scene demonstrates:
- Valid hotspots
- Empty IDs
- Invalid dimensions (0 or negative)
- Duplicate IDs
- Negative positions

Load this scene to see validation in action.

## Implementation Details

### Files Modified
- `Editor/MainWindow.xaml`: Added validation panel and status indicators
- `Editor/MainWindow.xaml.cs`: Implemented validation workflow
- `Editor/SceneValidator.cs`: Enhanced with auto-fix capabilities
- `Editor/ValidationIssueViewModel.cs`: UI binding for validation issues

### Architecture
The validation system uses a three-layer approach:
1. **Validation Logic** (`SceneValidator`): Pure validation and auto-fix logic
2. **ViewModel** (`ValidationIssueViewModel`): UI representation of issues
3. **UI Integration** (`MainWindow`): User interaction and workflow

This separation ensures validation logic can be reused in other contexts (e.g., CLI tools, automated testing).
