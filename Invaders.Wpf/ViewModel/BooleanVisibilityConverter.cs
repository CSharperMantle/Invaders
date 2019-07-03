using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Invaders.Wpf.Commons;

namespace Invaders.Wpf.ViewModel
{
    public sealed class BooleanVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool) value)
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Two-way binding is not supported on " +
                                            nameof(BooleanVisibilityConverter));
        }
    }
}