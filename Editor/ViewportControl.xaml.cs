using Engine.Common;
using Engine.Data.Scene;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Editor
{
    public partial class ViewportControl : UserControl
    {
        private SceneDTO? _currentScene;
        private Point _lastMousePos;
        private bool _isPanning;
        private bool _isDraggingHotspot;
        private bool _isResizingHotspot;
        private HotspotDTO? _selectedHotspot;
        private HotspotDTO? _draggedHotspot;
        private ResizeHandle _resizeHandle = ResizeHandle.None;
        private Point _dragStartPos;
        private RectF? _dragStartRect;
        
        private const double MIN_ZOOM = 0.1;
        private const double MAX_ZOOM = 5.0;
        private const double ZOOM_STEP = 0.1;
        private const int DEFAULT_GRID_SIZE = 16;
        
        private int _gridSize = DEFAULT_GRID_SIZE;
        
        public event EventHandler<HotspotDTO?>? HotspotSelected;

        private enum ResizeHandle
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Top,
            Right,
            Bottom,
            Left
        }

        public ViewportControl()
        {
            InitializeComponent();
            Loaded += (s, e) => Focus();
        }

        public void SetScene(SceneDTO? scene, string? projectBasePath = null)
        {
            _currentScene = scene;
            _selectedHotspot = null;
            
            if (scene == null)
            {
                BackgroundImage.Source = null;
                HotspotsCanvas.Children.Clear();
                return;
            }

            LoadBackground(scene, projectBasePath);
            RenderHotspots();
            
            // Reset view
            ResetView();
        }

        public void SetSelectedHotspot(HotspotDTO? hotspot)
        {
            _selectedHotspot = hotspot;
            RenderHotspots();
        }

        public void RefreshHotspots()
        {
            RenderHotspots();
        }

        private void LoadBackground(SceneDTO scene, string? projectBasePath)
        {
            if (string.IsNullOrWhiteSpace(scene.BackgroundPath))
            {
                BackgroundImage.Source = null;
                return;
            }

            try
            {
                string imagePath;
                
                // Try to find the image in the Game/Content directory
                if (!string.IsNullOrWhiteSpace(projectBasePath))
                {
                    imagePath = Path.Combine(projectBasePath, "Content", scene.BackgroundPath);
                }
                else
                {
                    // Fallback: relative to editor
                    var basePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Game", "Content");
                    imagePath = Path.Combine(basePath, scene.BackgroundPath);
                }
                
                imagePath = Path.GetFullPath(imagePath);
                
                if (File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    BackgroundImage.Source = bitmap;
                }
                else
                {
                    BackgroundImage.Source = null;
                }
            }
            catch
            {
                BackgroundImage.Source = null;
            }
        }

        private void RenderHotspots()
        {
            HotspotsCanvas.Children.Clear();
            
            if (_currentScene == null) return;

            foreach (var hotspot in _currentScene.Hotspots)
            {
                bool isSelected = hotspot == _selectedHotspot;
                DrawHotspot(hotspot, isSelected);
            }
        }

        private void DrawHotspot(HotspotDTO hotspot, bool isSelected)
        {
            var rect = new System.Windows.Shapes.Rectangle
            {
                Width = hotspot.Rect.Width,
                Height = hotspot.Rect.Height,
                Stroke = isSelected ? Brushes.Yellow : Brushes.Cyan,
                StrokeThickness = isSelected ? 2 : 1,
                Fill = isSelected ? new SolidColorBrush(Color.FromArgb(40, 255, 255, 0)) 
                                  : new SolidColorBrush(Color.FromArgb(20, 0, 255, 255)),
                Tag = hotspot
            };

            Canvas.SetLeft(rect, hotspot.Rect.X);
            Canvas.SetTop(rect, hotspot.Rect.Y);
            HotspotsCanvas.Children.Add(rect);

            // Add label
            var label = new TextBlock
            {
                Text = hotspot.Id,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                Padding = new Thickness(4, 2, 4, 2),
                FontSize = 11,
                Tag = hotspot
            };
            
            Canvas.SetLeft(label, hotspot.Rect.X);
            Canvas.SetTop(label, hotspot.Rect.Y - 20);
            HotspotsCanvas.Children.Add(label);

            // Draw resize handles if selected
            if (isSelected)
            {
                DrawResizeHandles(hotspot);
            }
        }

        private void DrawResizeHandles(HotspotDTO hotspot)
        {
            var handleSize = 8.0 / ZoomTransform.ScaleX; // Scale-independent size
            var handles = new[]
            {
                (ResizeHandle.TopLeft, hotspot.Rect.X, hotspot.Rect.Y),
                (ResizeHandle.TopRight, hotspot.Rect.X + hotspot.Rect.Width, hotspot.Rect.Y),
                (ResizeHandle.BottomLeft, hotspot.Rect.X, hotspot.Rect.Y + hotspot.Rect.Height),
                (ResizeHandle.BottomRight, hotspot.Rect.X + hotspot.Rect.Width, hotspot.Rect.Y + hotspot.Rect.Height),
                (ResizeHandle.Top, hotspot.Rect.X + hotspot.Rect.Width / 2, hotspot.Rect.Y),
                (ResizeHandle.Right, hotspot.Rect.X + hotspot.Rect.Width, hotspot.Rect.Y + hotspot.Rect.Height / 2),
                (ResizeHandle.Bottom, hotspot.Rect.X + hotspot.Rect.Width / 2, hotspot.Rect.Y + hotspot.Rect.Height),
                (ResizeHandle.Left, hotspot.Rect.X, hotspot.Rect.Y + hotspot.Rect.Height / 2)
            };

            foreach (var (handle, x, y) in handles)
            {
                var handleRect = new System.Windows.Shapes.Rectangle
                {
                    Width = handleSize,
                    Height = handleSize,
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Tag = new Tuple<HotspotDTO, ResizeHandle>(hotspot, handle)
                };

                Canvas.SetLeft(handleRect, x - handleSize / 2);
                Canvas.SetTop(handleRect, y - handleSize / 2);
                HotspotsCanvas.Children.Add(handleRect);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Zoom with mouse wheel
            var delta = e.Delta > 0 ? ZOOM_STEP : -ZOOM_STEP;
            var newZoom = Math.Clamp(ZoomTransform.ScaleX + delta, MIN_ZOOM, MAX_ZOOM);
            
            // Zoom towards mouse position
            var mousePos = e.GetPosition(ViewportCanvas);
            var beforeZoom = new Point(
                (mousePos.X - PanTransform.X) / ZoomTransform.ScaleX,
                (mousePos.Y - PanTransform.Y) / ZoomTransform.ScaleY
            );
            
            ZoomTransform.ScaleX = newZoom;
            ZoomTransform.ScaleY = newZoom;
            
            var afterZoom = new Point(
                beforeZoom.X * ZoomTransform.ScaleX,
                beforeZoom.Y * ZoomTransform.ScaleY
            );
            
            PanTransform.X += mousePos.X - afterZoom.X - PanTransform.X;
            PanTransform.Y += mousePos.Y - afterZoom.Y - PanTransform.Y;
            
            UpdateZoomDisplay();
            e.Handled = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            
            var pos = e.GetPosition(ViewportCanvas);
            var worldPos = TransformToWorld(pos);
            
            if (e.MiddleButton == MouseButtonState.Pressed || 
                (e.RightButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.None))
            {
                // Start panning
                _isPanning = true;
                _lastMousePos = pos;
                Mouse.Capture(this);
                e.Handled = true;
                return;
            }
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Check for resize handle click
                if (TryGetResizeHandle(pos, out var hotspot, out var handle))
                {
                    _isResizingHotspot = true;
                    _draggedHotspot = hotspot;
                    _resizeHandle = handle;
                    _dragStartPos = worldPos;
                    _dragStartRect = new RectF 
                    { 
                        X = hotspot.Rect.X, 
                        Y = hotspot.Rect.Y, 
                        Width = hotspot.Rect.Width, 
                        Height = hotspot.Rect.Height 
                    };
                    Mouse.Capture(this);
                    e.Handled = true;
                    return;
                }
                
                // Check for hotspot click
                var clickedHotspot = HitTestHotspot(worldPos);
                if (clickedHotspot != null)
                {
                    if (_selectedHotspot == clickedHotspot)
                    {
                        // Start dragging
                        _isDraggingHotspot = true;
                        _draggedHotspot = clickedHotspot;
                        _dragStartPos = worldPos;
                        _dragStartRect = new RectF 
                        { 
                            X = clickedHotspot.Rect.X, 
                            Y = clickedHotspot.Rect.Y, 
                            Width = clickedHotspot.Rect.Width, 
                            Height = clickedHotspot.Rect.Height 
                        };
                        Mouse.Capture(this);
                    }
                    else
                    {
                        // Select hotspot
                        _selectedHotspot = clickedHotspot;
                        RenderHotspots();
                        HotspotSelected?.Invoke(this, clickedHotspot);
                    }
                    e.Handled = true;
                    return;
                }
                
                // Clicked on empty area - deselect
                if (_selectedHotspot != null)
                {
                    _selectedHotspot = null;
                    RenderHotspots();
                    HotspotSelected?.Invoke(this, null);
                    e.Handled = true;
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(ViewportCanvas);
            
            if (_isPanning)
            {
                var delta = pos - _lastMousePos;
                PanTransform.X += delta.X;
                PanTransform.Y += delta.Y;
                _lastMousePos = pos;
                e.Handled = true;
                return;
            }
            
            if (_isDraggingHotspot && _draggedHotspot != null && _dragStartRect != null)
            {
                var worldPos = TransformToWorld(pos);
                var deltaX = (float)(worldPos.X - _dragStartPos.X);
                var deltaY = (float)(worldPos.Y - _dragStartPos.Y);
                
                var newX = _dragStartRect.X + deltaX;
                var newY = _dragStartRect.Y + deltaY;
                
                if (SnapToGridCheckBox.IsChecked == true)
                {
                    newX = SnapToGrid(newX);
                    newY = SnapToGrid(newY);
                }
                
                _draggedHotspot.Rect.X = newX;
                _draggedHotspot.Rect.Y = newY;
                RenderHotspots();
                e.Handled = true;
                return;
            }
            
            if (_isResizingHotspot && _draggedHotspot != null && _dragStartRect != null)
            {
                var worldPos = TransformToWorld(pos);
                var deltaX = (float)(worldPos.X - _dragStartPos.X);
                var deltaY = (float)(worldPos.Y - _dragStartPos.Y);
                
                ResizeHotspot(_draggedHotspot, _resizeHandle, deltaX, deltaY);
                RenderHotspots();
                e.Handled = true;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning || _isDraggingHotspot || _isResizingHotspot)
            {
                Mouse.Capture(null);
                _isPanning = false;
                _isDraggingHotspot = false;
                _isResizingHotspot = false;
                _draggedHotspot = null;
                _dragStartRect = null;
                _resizeHandle = ResizeHandle.None;
                e.Handled = true;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_isPanning || _isDraggingHotspot || _isResizingHotspot)
            {
                Mouse.Capture(null);
                _isPanning = false;
                _isDraggingHotspot = false;
                _isResizingHotspot = false;
                _draggedHotspot = null;
                _dragStartRect = null;
                _resizeHandle = ResizeHandle.None;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                // Zoom in
                ZoomTransform.ScaleX = Math.Min(ZoomTransform.ScaleX + ZOOM_STEP, MAX_ZOOM);
                ZoomTransform.ScaleY = ZoomTransform.ScaleX;
                UpdateZoomDisplay();
                e.Handled = true;
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                // Zoom out
                ZoomTransform.ScaleX = Math.Max(ZoomTransform.ScaleX - ZOOM_STEP, MIN_ZOOM);
                ZoomTransform.ScaleY = ZoomTransform.ScaleX;
                UpdateZoomDisplay();
                e.Handled = true;
            }
            else if (e.Key == Key.D0 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Reset zoom
                ResetView();
                e.Handled = true;
            }
            else if (e.Key == Key.G && Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Toggle grid
                SnapToGridCheckBox.IsChecked = !SnapToGridCheckBox.IsChecked;
                e.Handled = true;
            }
        }

        private void OnSnapToGridChanged(object sender, RoutedEventArgs e)
        {
            GridSizeText.Visibility = SnapToGridCheckBox.IsChecked == true 
                ? Visibility.Visible 
                : Visibility.Collapsed;
            
            if (SnapToGridCheckBox.IsChecked == true)
            {
                DrawGrid();
            }
            else
            {
                GridCanvas.Children.Clear();
            }
        }

        private void DrawGrid()
        {
            GridCanvas.Children.Clear();
            
            if (BackgroundImage.Source == null) return;
            
            var width = BackgroundImage.Source.Width;
            var height = BackgroundImage.Source.Height;
            
            // Draw vertical lines
            for (double x = 0; x < width; x += _gridSize)
            {
                var line = new Line
                {
                    X1 = x * ZoomTransform.ScaleX + PanTransform.X,
                    Y1 = 0,
                    X2 = x * ZoomTransform.ScaleX + PanTransform.X,
                    Y2 = ActualHeight,
                    Stroke = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                    StrokeThickness = 1
                };
                GridCanvas.Children.Add(line);
            }
            
            // Draw horizontal lines
            for (double y = 0; y < height; y += _gridSize)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y * ZoomTransform.ScaleY + PanTransform.Y,
                    X2 = ActualWidth,
                    Y2 = y * ZoomTransform.ScaleY + PanTransform.Y,
                    Stroke = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                    StrokeThickness = 1
                };
                GridCanvas.Children.Add(line);
            }
        }

        private void ResizeHotspot(HotspotDTO hotspot, ResizeHandle handle, float deltaX, float deltaY)
        {
            if (_dragStartRect == null) return;
            
            var newRect = new RectF
            {
                X = _dragStartRect.X,
                Y = _dragStartRect.Y,
                Width = _dragStartRect.Width,
                Height = _dragStartRect.Height
            };
            
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    newRect.X = _dragStartRect.X + deltaX;
                    newRect.Y = _dragStartRect.Y + deltaY;
                    newRect.Width = _dragStartRect.Width - deltaX;
                    newRect.Height = _dragStartRect.Height - deltaY;
                    break;
                case ResizeHandle.TopRight:
                    newRect.Y = _dragStartRect.Y + deltaY;
                    newRect.Width = _dragStartRect.Width + deltaX;
                    newRect.Height = _dragStartRect.Height - deltaY;
                    break;
                case ResizeHandle.BottomLeft:
                    newRect.X = _dragStartRect.X + deltaX;
                    newRect.Width = _dragStartRect.Width - deltaX;
                    newRect.Height = _dragStartRect.Height + deltaY;
                    break;
                case ResizeHandle.BottomRight:
                    newRect.Width = _dragStartRect.Width + deltaX;
                    newRect.Height = _dragStartRect.Height + deltaY;
                    break;
                case ResizeHandle.Top:
                    newRect.Y = _dragStartRect.Y + deltaY;
                    newRect.Height = _dragStartRect.Height - deltaY;
                    break;
                case ResizeHandle.Right:
                    newRect.Width = _dragStartRect.Width + deltaX;
                    break;
                case ResizeHandle.Bottom:
                    newRect.Height = _dragStartRect.Height + deltaY;
                    break;
                case ResizeHandle.Left:
                    newRect.X = _dragStartRect.X + deltaX;
                    newRect.Width = _dragStartRect.Width - deltaX;
                    break;
            }
            
            // Ensure minimum size
            if (newRect.Width < 10) newRect.Width = 10;
            if (newRect.Height < 10) newRect.Height = 10;
            
            if (SnapToGridCheckBox.IsChecked == true)
            {
                newRect.X = SnapToGrid(newRect.X);
                newRect.Y = SnapToGrid(newRect.Y);
                newRect.Width = SnapToGrid(newRect.Width);
                newRect.Height = SnapToGrid(newRect.Height);
            }
            
            hotspot.Rect = newRect;
        }

        private bool TryGetResizeHandle(Point screenPos, out HotspotDTO? hotspot, out ResizeHandle handle)
        {
            hotspot = null;
            handle = ResizeHandle.None;
            
            if (_selectedHotspot == null) return false;
            
            var worldPos = TransformToWorld(screenPos);
            var handleSize = 8.0 / ZoomTransform.ScaleX;
            
            var handles = new[]
            {
                (ResizeHandle.TopLeft, _selectedHotspot.Rect.X, _selectedHotspot.Rect.Y),
                (ResizeHandle.TopRight, _selectedHotspot.Rect.X + _selectedHotspot.Rect.Width, _selectedHotspot.Rect.Y),
                (ResizeHandle.BottomLeft, _selectedHotspot.Rect.X, _selectedHotspot.Rect.Y + _selectedHotspot.Rect.Height),
                (ResizeHandle.BottomRight, _selectedHotspot.Rect.X + _selectedHotspot.Rect.Width, _selectedHotspot.Rect.Y + _selectedHotspot.Rect.Height),
                (ResizeHandle.Top, _selectedHotspot.Rect.X + _selectedHotspot.Rect.Width / 2, _selectedHotspot.Rect.Y),
                (ResizeHandle.Right, _selectedHotspot.Rect.X + _selectedHotspot.Rect.Width, _selectedHotspot.Rect.Y + _selectedHotspot.Rect.Height / 2),
                (ResizeHandle.Bottom, _selectedHotspot.Rect.X + _selectedHotspot.Rect.Width / 2, _selectedHotspot.Rect.Y + _selectedHotspot.Rect.Height),
                (ResizeHandle.Left, _selectedHotspot.Rect.X, _selectedHotspot.Rect.Y + _selectedHotspot.Rect.Height / 2)
            };
            
            foreach (var (h, x, y) in handles)
            {
                var dist = Math.Sqrt(Math.Pow(worldPos.X - x, 2) + Math.Pow(worldPos.Y - y, 2));
                if (dist <= handleSize)
                {
                    hotspot = _selectedHotspot;
                    handle = h;
                    return true;
                }
            }
            
            return false;
        }

        private HotspotDTO? HitTestHotspot(Point worldPos)
        {
            if (_currentScene == null) return null;
            
            // Check in reverse order (top to bottom)
            for (int i = _currentScene.Hotspots.Count - 1; i >= 0; i--)
            {
                var hs = _currentScene.Hotspots[i];
                if (worldPos.X >= hs.Rect.X && worldPos.X <= hs.Rect.X + hs.Rect.Width &&
                    worldPos.Y >= hs.Rect.Y && worldPos.Y <= hs.Rect.Y + hs.Rect.Height)
                {
                    return hs;
                }
            }
            
            return null;
        }

        private Point TransformToWorld(Point screenPos)
        {
            return new Point(
                (screenPos.X - PanTransform.X) / ZoomTransform.ScaleX,
                (screenPos.Y - PanTransform.Y) / ZoomTransform.ScaleY
            );
        }

        private float SnapToGrid(float value)
        {
            return (float)(Math.Round(value / _gridSize) * _gridSize);
        }

        private void UpdateZoomDisplay()
        {
            ZoomLevelText.Text = $"Zoom: {(int)(ZoomTransform.ScaleX * 100)}%";
            
            if (SnapToGridCheckBox.IsChecked == true)
            {
                DrawGrid();
            }
        }

        private void ResetView()
        {
            ZoomTransform.ScaleX = 1.0;
            ZoomTransform.ScaleY = 1.0;
            PanTransform.X = 0;
            PanTransform.Y = 0;
            UpdateZoomDisplay();
        }
    }
}
