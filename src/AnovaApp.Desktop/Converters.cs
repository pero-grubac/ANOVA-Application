using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AnovaApp.Desktop;

/// <summary>
/// Shows an element when the value is "present": <see langword="true"/>, or any non-null object.
/// With <c>ConverterParameter=Invert</c> it does the opposite.
/// </summary>
public sealed class VisibleWhenConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool present = value is bool b ? b : value is not null;
        if (parameter is "Invert")
            present = !present;
        return present ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
