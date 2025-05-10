using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Unused helper.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}