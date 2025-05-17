using System.Globalization;
using System.Windows.Data;

namespace APKTool.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter == null)
            return false;

        string enumValue = value.ToString();
        string targetValue = parameter.ToString();
        return enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return Enum.Parse(targetType, parameter.ToString());

        return Binding.DoNothing;
    }
}
