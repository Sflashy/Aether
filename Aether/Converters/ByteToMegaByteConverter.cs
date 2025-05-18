using System.Globalization;
using System.Windows.Data;

namespace Aether.Converters
{
    public class ByteToMegaByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int size = (int)value;
            double megabyte = size / (1024.0 * 1024.0);
            return $"{megabyte:F2} MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
