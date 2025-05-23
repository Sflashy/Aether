﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Aether.Converters;

public class USBDebuggingStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            switch (status)
            {
                case "Enabled":
                    return new SolidColorBrush(Color.FromArgb(255, 0, 255, 159));
                case "Disabled":
                    return new SolidColorBrush(Color.FromArgb(255, 244, 71, 71));
                default:
                    return new SolidColorBrush(Color.FromRgb(224, 224, 224));
            }
        }
        return new SolidColorBrush(Color.FromRgb(224, 224, 224));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
