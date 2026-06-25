using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using StockWise.App.Models;

namespace StockWise.App.Converters;

public class ToastTypeToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ToastType.Success => new SolidColorBrush(Color.Parse("#22C55E")),
            ToastType.Error => new SolidColorBrush(Color.Parse("#EF4444")),
            ToastType.Warning => new SolidColorBrush(Color.Parse("#F59E0B")),
            ToastType.Info => new SolidColorBrush(Color.Parse("#06B6D4")),
            _ => new SolidColorBrush(Color.Parse("#06B6D4")),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
