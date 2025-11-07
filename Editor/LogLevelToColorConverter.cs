using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Engine.Diagnostics;

namespace Editor
{
    /// <summary>
    /// Converts LogLevel to a color for display.
    /// </summary>
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                return level switch
                {
                    LogLevel.Debug => new SolidColorBrush(Color.FromRgb(128, 128, 128)), // Gray
                    LogLevel.Info => new SolidColorBrush(Color.FromRgb(255, 255, 255)),  // White
                    LogLevel.Warn => new SolidColorBrush(Color.FromRgb(255, 215, 0)),    // Gold/Yellow
                    LogLevel.Error => new SolidColorBrush(Color.FromRgb(255, 100, 100)), // Red
                    _ => new SolidColorBrush(Color.FromRgb(255, 255, 255))
                };
            }
            return new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
