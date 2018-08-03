using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Invaders.Uwp.ViewModel
{
    public sealed class BooleanVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool && (bool) value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException("Two-way binding is not supported on " +
                                            nameof(BooleanVisibilityConverter));
        }
    }
}