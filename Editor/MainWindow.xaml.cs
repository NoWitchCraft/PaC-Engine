using Engine.Data.Scene;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Editor
{
    public partial class MainWindow : Window
    {
        private SceneDTO? _currentScene;
        private string? _currentScenePath;

        private HashSet<string> _expandedNames = new();
        
        private LogPanel? _logPanel;

        private enum TreeKind { Scene, HotspotGroup, Hotspot, EntitiesGroup }

        private class EngineTreeItem
        {
            public string Name { get; set; } = "";
            public TreeKind Kind {  get; set; }
            public object? Payload { get; set; }
            public List<EngineTreeItem> Children { get; set; } = new();
            public override string ToString() => Name;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private string GenerateUniqueHotspotId(SceneDTO scene)
        {
            string baseId = "hotspot";
            if (!scene.Hotspots.Any(h => h.Id.Equals(baseId, StringComparison.OrdinalIgnoreCase)))
                return baseId;

            int n = 1;
            while (scene.Hotspots.Any(h => h.Id.Equals($"{baseId}{n}", StringComparison.OrdinalIgnoreCase)))
                n++;
            return $"{baseId}{n}";
        }

        private void CtxAddHotspot_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null) return;
            if (sender is FrameworkElement fe && fe.DataContext is EngineTreeItem ti && ti.Kind == TreeKind.HotspotGroup)
            {
                var id = GenerateUniqueHotspotId(_currentScene);
                var hs = new HotspotDTO
                {
                    Id = id,
                    LabelKey = id,
                    Rect = new Engine.Common.RectF { X = 64, Y = 64, Width = 160, Height = 80 },
                    Highlight = true
                };
                _currentScene.Hotspots.Add(hs);
                RefreshHierarchyPreserveSelection(hs);
                InspectorHost.Content = hs;
                StatusText.Text = $"Hotspot added: {hs.Id}";
            }
        }

        private void HierarchyTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject? d = e.OriginalSource as DependencyObject;
            while (d != null && d is not TreeViewItem)
                d = VisualTreeHelper.GetParent(d);

            if (d is TreeViewItem tvi)
            {
                tvi.IsSelected = true;
                tvi.Focus();
            }
        }

        private void CtxDeleteHotspot_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null) return;
            if (sender is FrameworkElement fe && fe.DataContext is EngineTreeItem ti && ti.Kind == TreeKind.Hotspot && ti.Payload is HotspotDTO hs)
            {
                var confirm = MessageBox.Show(this, $"Delete hotspot '{hs.Id}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm != MessageBoxResult.Yes) return;

                _currentScene.Hotspots.Remove(hs);
                InspectorHost.Content = _currentScene;
                RefreshHierarchyPreserveSelection(_currentScene);
                StatusText.Text = $"Hotspot deleted: {hs.Id}";
            }
        }

        private void AddHotspotMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null)
            {
                MessageBox.Show(this, "Open a scene first.", "Add Hotspot",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var id = GenerateUniqueHotspotId(_currentScene);
            var hs = new HotspotDTO
            {
                Id = id,
                LabelKey = id,
                Rect = new Engine.Common.RectF { X = 64, Y = 64, Width = 160, Height = 80 },
                EventPathId = null,
                CursorId = null,
                Highlight = true
            };

            _currentScene.Hotspots.Add(hs);

            // Tree aktualisieren & direkt den neuen Hotspot im Inspector auswählen
            RefreshHierarchyPreserveSelection(hs);
            InspectorHost.Content = hs;
            StatusText.Text = $"Hotspot added: {hs.Id}";
        }

        private void DeleteHotspot_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null) return;

            // Welcher Hotspot? -> steckt im Button.Tag
            if (sender is FrameworkElement fe && fe.Tag is HotspotDTO hs)
            {
                var confirm = MessageBox.Show(this,
                    $"Delete hotspot '{hs.Id}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm != MessageBoxResult.Yes) return;

                // Entfernen
                _currentScene.Hotspots.Remove(hs);

                // Inspector zurück auf Scene, Tree refreshen
                InspectorHost.Content = _currentScene;
                RefreshHierarchyPreserveSelection(_currentScene);
                StatusText.Text = $"Hotspot deleted: {hs.Id}";
            }
        }

        private void CaptureExpansion()
        {
            _expandedNames.Clear();
            foreach (var item in HierarchyTree.Items)
                CaptureExpansionRecursive(HierarchyTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem);
        }

        private void CaptureExpansionRecursive(TreeViewItem? tvi)
        {
            if (tvi == null) return;

            if (tvi.IsExpanded)
            {
                if (tvi.Header is EngineTreeItem eti)
                    _expandedNames.Add(eti.Name);
                else if (tvi.Header != null)
                    _expandedNames.Add(tvi.Header.ToString()!);
            }

            foreach (var child in tvi.Items) // ItemCollection ist IEnumerable
            {
                var c = tvi.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                CaptureExpansionRecursive(c);
            }
        }

        // stellt expandierte Knoten wieder her
        private void RestoreExpansion()
        {
            foreach (var item in HierarchyTree.Items)
                RestoreExpansionRecursive(HierarchyTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem);
        }

        private void RestoreExpansionRecursive(TreeViewItem? tvi)
        {
            if (tvi == null) return;

            if (tvi.Header is EngineTreeItem eti && _expandedNames.Contains(eti.Name))
                tvi.IsExpanded = true;

            foreach (var child in tvi.Items)
            {
                var c = tvi.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                RestoreExpansionRecursive(c);
            }
        }


        private void RefreshHierarchyPreserveSelection(object? currentSelection = null)
        {
            if (_currentScene == null) return;
            currentSelection ??= InspectorHost.Content;

            // Expansion & Auswahl merken
            CaptureExpansion();

            BuildHierarchy(_currentScene);

            // UI-Thread warten, bis Container gebaut sind, dann wiederherstellen
            HierarchyTree.Dispatcher.InvokeAsync(() =>
            {
                RestoreExpansion();
                InspectorHost.Content = currentSelection;
            });
            StatusText.Text = $"Updated: {_currentScene.Id}";
        }

        private void SceneId_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshHierarchyPreserveSelection(_currentScene);
        }

        private void HotspotId_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshHierarchyPreserveSelection(InspectorHost.Content);
        }

        private void OpenSceneMenu_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open Scene (.scene.json)",
                Filter = "Scene files (*.scene.json)|*.scene.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
                CheckFileExists = true
            };

            try
            {
                var guess = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Game", "Content", "Scenes");
                var full = Path.GetFullPath(guess);
                if (Directory.Exists(full))
                    dlg.InitialDirectory = full;
            }
            catch { /* ignore */ }

            if (dlg.ShowDialog() == true)
            {
                LoadSceneFromFile(dlg.FileName);
            }
        }

        private void LoadSceneFromFile(string absolutePath)
        {
            try
            {
                var scene = SceneIO.Load(absolutePath);
                _currentScene = scene;
                _currentScenePath = absolutePath;

                Title = $"PaCEngine Editor – {scene.Id}";
                StatusText.Text = $"Loaded: {scene.Id} | BG={scene.BackgroundPath} | Hotspots={scene.Hotspots.Count}";

                BuildHierarchy(scene);
                // Inspector zeigt zunächst die Scene selbst
                InspectorHost.Content = scene;
                var vr = SceneValidator.Validate(scene);
                ShowValidationStatus(vr, prefix: "Loaded");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"Could not load scene:\n{ex.Message}",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Load failed.";
            }
        }

        private void BuildHierarchy(SceneDTO scene)
        {
            var root = new EngineTreeItem { Name = $"Scene: {scene.Id}", Kind = TreeKind.Scene, Payload = scene };

            var hsGroup = new EngineTreeItem { Name = $"Hotspots ({scene.Hotspots.Count})", Kind = TreeKind.HotspotGroup };
            foreach (var hs in scene.Hotspots)
                hsGroup.Children.Add(new EngineTreeItem { Name = $"Hotspot: {hs.Id}", Kind = TreeKind.Hotspot, Payload = hs });
            root.Children.Add(hsGroup);

            var entGroup = new EngineTreeItem { Name = "Sprites/Entities (0)", Kind = TreeKind.EntitiesGroup };
            root.Children.Add(entGroup);

            HierarchyTree.ItemsSource = new[] { root };
        }


        private void HierarchyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is EngineTreeItem ti)
                InspectorHost.Content = ti.Payload ?? _currentScene;
        }

        private void SaveSceneMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null)
            {
                MessageBox.Show(this, "No scene loaded.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(_currentScenePath))
            {
                SaveSceneAsMenu_Click(sender, e);
                return;
            }

            TrySaveTo(_currentScenePath);
        }

        private void SaveSceneAsMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null)
            {
                MessageBox.Show(this, "No scene loaded.", "Save As", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Save Scene As",
                Filter = "Scene files (*.scene.json)|*.scene.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = !string.IsNullOrWhiteSpace(_currentScenePath)
                           ? Path.GetFileName(_currentScenePath)
                           : $"{_currentScene.Id}.scene.json"
            };

            try
            {
                var guess = !string.IsNullOrWhiteSpace(_currentScenePath)
                            ? Path.GetDirectoryName(_currentScenePath)!
                            : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Game", "Content", "Scenes");
                var full = Path.GetFullPath(guess);
                if (Directory.Exists(full))
                    dlg.InitialDirectory = full;
            }
            catch { /* ignore */ }

            if (dlg.ShowDialog() == true)
            {
                TrySaveTo(dlg.FileName);
            }
        }

        private void TrySaveTo(string absolutePath)
        {
            try
            {
                if (_currentScene != null && string.IsNullOrWhiteSpace(_currentScene.Id))
                {
                    _currentScene.Id = Path.GetFileNameWithoutExtension(absolutePath)
                        .Replace(".scene", "", StringComparison.OrdinalIgnoreCase);
                }

                SceneIO.Save(_currentScene!, absolutePath);
                _currentScenePath = absolutePath;

                Title = $"PaCEngine Editor – {_currentScene!.Id}";
                StatusText.Text = $"Saved: {_currentScene.Id} → {absolutePath}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"Could not save scene:\n{ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Save failed.";
            }
        }

        //Validation:

        private void ValidateSceneMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScene == null)
            {
                MessageBox.Show(this, "No scene loaded.", "Validate", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var vr = SceneValidator.Validate(_currentScene);
            ShowValidationStatus(vr, prefix: "Validated");
        }

        private void ShowValidationStatus(ValidationResult vr, string prefix)
        {
            if (vr.HasErrors)
            {
                StatusText.Text = $"{prefix}: {vr.Issues.Count} issues – {vr.Issues.Count(i => i.Severity == IssueSeverity.Error)} error(s), {vr.Issues.Count(i => i.Severity == IssueSeverity.Warning)} warning(s)";
                StatusText.Foreground = System.Windows.Media.Brushes.OrangeRed;

                // Optional: Detaildialog bei Fehlern
                var details = string.Join(Environment.NewLine, vr.Issues.Select(i => i.ToString()));
                MessageBox.Show(this, details, "Validation Errors/Warnings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (vr.HasWarnings)
            {
                StatusText.Text = $"{prefix}: {vr.Issues.Count} issues – 0 errors, {vr.Issues.Count(i => i.Severity == IssueSeverity.Warning)} warning(s)";
                StatusText.Foreground = System.Windows.Media.Brushes.DarkGoldenrod;
            }
            else
            {
                StatusText.Text = $"{prefix}: OK";
                StatusText.Foreground = System.Windows.Media.Brushes.SeaGreen;
            }
        }


        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogViewerMenu_Click(object sender, RoutedEventArgs e)
        {
            if (App.EventLogger == null)
            {
                MessageBox.Show(this, "Event logger is not initialized.", "Log Viewer", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_logPanel == null || !_logPanel.IsLoaded)
            {
                _logPanel = new LogPanel(App.EventLogger);
                _logPanel.Owner = this;
            }

            _logPanel.Show();
            _logPanel.Activate();
        }
    }

}
