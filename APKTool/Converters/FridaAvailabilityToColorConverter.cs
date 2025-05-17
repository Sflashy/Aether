using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace APKTool.Converters;

public class FridaAvailabilityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Boolean) return new SolidColorBrush(Color.FromRgb(224, 224, 224));
        return (bool)value ? new SolidColorBrush(Color.FromArgb(255, 0, 255, 159)) : new SolidColorBrush(Color.FromArgb(255, 244, 71, 71));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
