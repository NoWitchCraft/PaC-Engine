using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Engine.Diagnostics;

namespace Editor
{
    /// <summary>
    /// ViewModel for the Log Panel.
    /// </summary>
    public class LogPanelViewModel : INotifyPropertyChanged
    {
        private readonly EventLogger _eventLogger;
        private readonly ObservableCollection<LogEntry> _allEntries = new ObservableCollection<LogEntry>();
        private ICollectionView? _filteredView;
        
        private LogLevel _filterLevel = LogLevel.Debug;
        private string _searchText = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICollectionView Entries
        {
            get
            {
                if (_filteredView == null)
                {
                    _filteredView = CollectionViewSource.GetDefaultView(_allEntries);
                    _filteredView.Filter = FilterEntry;
                }
                return _filteredView;
            }
        }

        public LogLevel FilterLevel
        {
            get => _filterLevel;
            set
            {
                if (_filterLevel != value)
                {
                    _filterLevel = value;
                    OnPropertyChanged(nameof(FilterLevel));
                    _filteredView?.Refresh();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    _filteredView?.Refresh();
                }
            }
        }

        public LogPanelViewModel(EventLogger eventLogger)
        {
            _eventLogger = eventLogger ?? throw new ArgumentNullException(nameof(eventLogger));
            
            // Subscribe to new log entries
            _eventLogger.LogEntryAdded += OnLogEntryAdded;

            // Load existing entries
            foreach (var entry in _eventLogger.GetEntries())
            {
                _allEntries.Add(entry);
            }
        }

        private void OnLogEntryAdded(object? sender, LogEntry entry)
        {
            // Add to collection on UI thread
            Application.Current?.Dispatcher.Invoke(() =>
            {
                _allEntries.Add(entry);
                
                // Limit to prevent memory issues
                while (_allEntries.Count > 10000)
                {
                    _allEntries.RemoveAt(0);
                }
            });
        }

        private bool FilterEntry(object obj)
        {
            if (obj is not LogEntry entry)
                return false;

            // Filter by level
            if (entry.Level < _filterLevel)
                return false;

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var search = _searchText.ToLowerInvariant();
                if (!entry.Source.ToLowerInvariant().Contains(search) &&
                    !entry.Message.ToLowerInvariant().Contains(search))
                    return false;
            }

            return true;
        }

        public void ClearLogs()
        {
            _allEntries.Clear();
            _eventLogger.Clear();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
