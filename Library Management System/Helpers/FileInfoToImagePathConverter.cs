using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Library_Management_System.Views.Pages
{
    /// <summary>
    /// This thing converts a <see cref="FileInfo"/> object to its full file path,
    /// which I use to display images in the UI.
    /// </summary>
    public class FileInfoToImagePathConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="FileInfo"/> object to a string representing the full file path.
        /// </summary>
        /// <param name="value">The value to convert, expected to be a <see cref="FileInfo"/> object.</param>
        /// <param name="targetType">The target type that the value is being converted to (typically a string).</param>
        /// <param name="parameter">Optional parameter that may be used to influence the conversion logic (ignore).</param>
        /// <param name="culture">The culture to use for the conversion (ignore).</param>
        /// <returns>The full file path of the <see cref="FileInfo"/> if the file exists; otherwise, null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileInfo fileInfo && fileInfo.Exists)
            {
                return fileInfo.FullName;
            }
            return null;
        }

        /// <summary>
        /// Converts a string file path back into a <see cref="FileInfo"/> object.
        /// </summary>
        /// <param name="value">The value to convert, expected to be a string (file path).</param>
        /// <param name="targetType">The target type that the value is being converted to (expected to be <see cref="FileInfo"/>).</param>
        /// <param name="parameter">Optional parameter that may be used to influence the conversion logic (not used here).</param>
        /// <param name="culture">The culture to use for the conversion (not used here).</param>
        /// <returns>Always returns null, as the backward conversion is not implemented.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO - No need right now
            return null;
        }
    }
}
