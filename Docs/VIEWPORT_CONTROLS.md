# Viewport Controls Documentation

## Overview
The Viewport is the central visual editing area in the PaC Engine Editor where you can see and manipulate the scene background and hotspots.

## Navigation Controls

### Zoom
- **Mouse Wheel**: Scroll up to zoom in, scroll down to zoom out
- **Plus Key (+)**: Zoom in
- **Minus Key (-)**: Zoom out
- **Ctrl+0**: Reset zoom to 100% and center view
- **Zoom Range**: 10% - 500%

### Pan
- **Middle Mouse Button**: Click and drag to pan the view
- **Right Mouse Button**: Click and drag to pan the view (alternative method)

## Hotspot Editing

### Selection
- **Left Click on Hotspot**: Select the hotspot (syncs with Hierarchy and Inspector)
- **Left Click on Empty Area**: Deselect current hotspot

### Moving Hotspots
1. Select a hotspot by clicking on it
2. Click and drag the hotspot to move it
3. The hotspot position updates in the Inspector in real-time

### Resizing Hotspots
1. Select a hotspot by clicking on it
2. Eight resize handles appear (corners and edges)
3. Click and drag any handle to resize the hotspot:
   - **Corner Handles**: Resize both width and height
   - **Edge Handles**: Resize only in one direction

### Snap to Grid
- **Checkbox**: Toggle "Snap to Grid" in the viewport overlay
- **Keyboard**: Press **Ctrl+G** to toggle snap-to-grid
- **Grid Size**: 16 pixels (configurable in code)
- When enabled, hotspot positions and sizes snap to the grid during move/resize operations

## Visual Indicators

### Hotspots
- **Unselected**: Cyan outline with semi-transparent fill
- **Selected**: Yellow outline with semi-transparent yellow fill
- **Label**: Hotspot ID displayed above each hotspot

### Resize Handles
- **Yellow Squares**: Appear on selected hotspot at corners and edge midpoints
- Size-independent (always visible regardless of zoom level)

### Grid Overlay
- **White Lines**: Semi-transparent grid lines when snap-to-grid is enabled
- Automatically adjusts to zoom level

## Integration with Other Panels

### Hierarchy Panel
- Clicking a hotspot in the Viewport selects it in the Hierarchy
- Selecting a hotspot in the Hierarchy highlights it in the Viewport

### Inspector Panel
- Selecting a hotspot in the Viewport shows its properties in the Inspector
- Changes to hotspot Rect properties (X, Y, Width, Height) in the Inspector update the Viewport
- Dragging/resizing in the Viewport updates Inspector values in real-time

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+S` | Save scene |
| `Ctrl+0` | Reset viewport (zoom 100%, center) |
| `Ctrl+G` | Toggle snap-to-grid |
| `+` / `-` | Zoom in / out |
| `Mouse Wheel` | Zoom in / out |

## Tips

1. **Precise Positioning**: Enable snap-to-grid for pixel-perfect alignment
2. **Quick Navigation**: Use middle mouse button for fast panning while working
3. **Multi-Monitor**: Zoom in to see details, zoom out for overview
4. **Keyboard Focus**: Click in the viewport to ensure keyboard shortcuts work
5. **Background Loading**: The background image is loaded from `Game/Content/{BackgroundPath}` relative to the editor

## Troubleshooting

### Background Not Showing
- Verify the `BackgroundPath` in the Scene Inspector is correct
- Ensure the image file exists in `Game/Content/` directory
- Check that the image format is supported (PNG, JPG, BMP)

### Hotspot Not Selectable
- Ensure you're clicking within the hotspot rectangle bounds
- Check that the hotspot is not hidden behind others (click multiple times to cycle selection)
- Verify the viewport has keyboard focus (click once in the viewport)

### Snap-to-Grid Not Working
- Verify the checkbox is enabled or press Ctrl+G
- Grid size is 16px by default - larger movements are more noticeable
- Snap only applies during drag/resize operations, not when editing values in Inspector
