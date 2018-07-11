using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Invaders.Wpf.ViewModel
{
    public class BooleanVisibiltyConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Two-way binding is not supported on " + nameof(BooleanVisibiltyConverter),
                new InvalidOperationException());
        }
    }
}