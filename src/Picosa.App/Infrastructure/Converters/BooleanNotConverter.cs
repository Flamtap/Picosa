using System;
using System.Globalization;
using System.Windows.Data;

namespace Picosa.App.Infrastructure.Converters
{
    public class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value as bool?) ?? value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value as bool?) ?? value;
        }
    }
}
