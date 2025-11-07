using System.Windows;
using Engine.Diagnostics;

namespace Editor
{
    /// <summary>
    /// Log Panel window for viewing and filtering log entries.
    /// </summary>
    public partial class LogPanel : Window
    {
        private readonly LogPanelViewModel _viewModel;

        public LogPanel(EventLogger eventLogger)
        {
            InitializeComponent();
            
            _viewModel = new LogPanelViewModel(eventLogger);
            DataContext = _viewModel;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearLogs();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
