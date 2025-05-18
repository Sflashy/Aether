using Aether.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aether.Converters;

public class ConsoleOutputColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ConsoleOutput output) return new SolidColorBrush(Color.FromRgb(224, 224, 224));
        
        switch (output.Type)
        {
            case ConsoleOutput.OutputType.Debug:
                return new SolidColorBrush(Color.FromRgb(224, 224, 224));
            case ConsoleOutput.OutputType.Info:
                return new SolidColorBrush(Color.FromRgb(102, 217, 239));
            case ConsoleOutput.OutputType.Warning:
                return new SolidColorBrush(Color.FromRgb(253, 151, 31));
            case ConsoleOutput.OutputType.Error:
                return new SolidColorBrush(Color.FromRgb(255, 85, 85));
            case ConsoleOutput.OutputType.Success:
                return new SolidColorBrush(Color.FromRgb(0, 255, 159));
            case ConsoleOutput.OutputType.Process:
                return new SolidColorBrush(Color.FromRgb(255, 255, 0));
            default:
                return new SolidColorBrush(Color.FromRgb(224, 224, 224));
        }
        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

