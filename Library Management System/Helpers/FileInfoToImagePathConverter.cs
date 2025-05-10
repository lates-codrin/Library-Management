using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Library_Management_System.Views.Pages
{
    public class FileInfoToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileInfo fileInfo && fileInfo.Exists)
            {
                return fileInfo.FullName;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
